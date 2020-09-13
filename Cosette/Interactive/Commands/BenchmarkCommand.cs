using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Xml.XPath;
using Cosette.Engine.Ai;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Fen;
using Cosette.Engine.Moves;
using Cosette.Engine.Moves.Magic;
using Cosette.Engine.Perft;
using Cosette.Engine.Perft.Results;

namespace Cosette.Interactive.Commands
{
    public class BenchmarkCommand : ICommand
    {
        public string Description { get; }
        private InteractiveConsole _interactiveConsole;

        public BenchmarkCommand(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;
            Description = "Test NegaMax performance using a few sample positions";
        }

        public void Run(params string[] parameters)
        {
            GC.TryStartNoGCRegion(1024 * 1024 * 16);

            var stopwatch = Stopwatch.StartNew();
            TestOpening();
            TestMidGame();
            TestEndGame();
            var total = stopwatch.Elapsed.TotalSeconds;

            _interactiveConsole.WriteLine($"Total time: {total:F} s");

            GC.EndNoGCRegion();
        }

        private void TestOpening()
        {
            var boardState = new BoardState();
            boardState.SetDefaultState();

            Test(boardState, "Opening", 11);
        }

        private void TestMidGame()
        {
            var boardState = FenParser.Parse("r2qr1k1/p2n1p2/1pb3pp/2ppN1P1/1R1PpP2/BQP1n1PB/P4N1P/1R4K1 w - - 0 21", out _);
            Test(boardState, "Midgame", 11);
        }

        private void TestEndGame()
        {
            var boardState = FenParser.Parse("7r/8/2k3P1/1p1p2Kp/1P6/2P5/7r/Q7 w - - 0 1", out _);
            Test(boardState, "Endgame", 16);
        }

        private void Test(BoardState boardState, string name, int depth)
        {
            _interactiveConsole.WriteLine($" == {name}:");

            TranspositionTable.Clear();
            IterativeDeepening.OnSearchUpdate += IterativeDeepening_OnOnSearchUpdate;
            IterativeDeepening.FindBestMove(boardState, 100_000, depth, 1);
            IterativeDeepening.OnSearchUpdate -= IterativeDeepening_OnOnSearchUpdate;

            _interactiveConsole.WriteLine();
        }

        private void IterativeDeepening_OnOnSearchUpdate(object? sender, SearchStatistics statistics)
        {
            // Main search result
            _interactiveConsole.WriteLine($"  === Depth: {statistics.Depth}, Score: {statistics.Score}, Best: {statistics.PrincipalVariation[0]}, " +
                                          $"Time: {((float) statistics.SearchTime / 1000):F} s");

            // Normal search
            _interactiveConsole.WriteLine($"   Normal search: Nodes: {statistics.Nodes}, Leafs: {statistics.Leafs}, " +
                                          $"Branching factor: {statistics.BranchingFactor:F}, Beta cutoffs: {statistics.BetaCutoffs}");

            // Quiescence search
            _interactiveConsole.WriteLine($"   Q search: Nodes: {statistics.QNodes}, Leafs: {statistics.QLeafs}, " +
                                          $"Branching factor: {statistics.QBranchingFactor:F}, Beta cutoffs: {statistics.QBetaCutoffs}");

            // Total
            _interactiveConsole.WriteLine($"   Total: Nodes: {statistics.TotalNodes} ({((float)statistics.TotalNodesPerSecond / 1000000):F} MN/s), " +
                                          $"Leafs: {statistics.TotalLeafs}, Branching factor: {statistics.TotalBranchingFactor:F}, " +
                                          $"Beta cutoffs: {statistics.TotalBetaCutoffs}");

            // Beta cutoffs at first move
            _interactiveConsole.WriteLine($"   Beta cutoffs at first move: {statistics.BetaCutoffsAtFirstMove} ({statistics.BetaCutoffsAtFirstMovePercent:F} %), " +
                                          $"Q Beta cutoffs at first move: {statistics.QBetaCutoffsAtFirstMove} ({statistics.QBetaCutoffsAtFirstMovePercent:F} %)");

            // Transposition statistics
            _interactiveConsole.WriteLine($"   Transposition: Entries: {statistics.TTEntries}, Hits: {statistics.TTHits} ({statistics.TTHitsPercent:F} %), " +
                                          $"NonHits: {statistics.TTNonHits}, Collisions: {statistics.TTCollisions}");

            _interactiveConsole.WriteLine();
        }
    }
}