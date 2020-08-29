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

        public BenchmarkCommand()
        {
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

            Console.WriteLine($"Total time: {total:F} s");

            GC.EndNoGCRegion();
        }

        private void TestOpening()
        {
            var boardState = new BoardState();
            boardState.SetDefaultState();

            Test(boardState, "Opening", 9);
        }

        private void TestMidGame()
        {
            var boardState = FenParser.Parse("r2qr1k1/p2n1p2/1pb3pp/2ppN1P1/1R1PpP2/BQP1n1PB/P4N1P/1R4K1 w - - 0 21", out _);
            Test(boardState, "Midgame", 7);
        }

        private void TestEndGame()
        {
            var boardState = FenParser.Parse("7r/8/2k3P1/1p1p2Kp/1P6/2P5/7r/Q7 w - - 0 1", out _);
            Test(boardState, "Endgame", 8);
        }

        private void Test(BoardState boardState, string name, int depth)
        {
            Console.WriteLine($" == {name}:");

            TranspositionTable.Clear();
            IterativeDeepening.OnSearchUpdate += IterativeDeepening_OnOnSearchUpdate;
            IterativeDeepening.FindBestMove(boardState, 100_000, 1);
            IterativeDeepening.OnSearchUpdate -= IterativeDeepening_OnOnSearchUpdate;
        }

        private void IterativeDeepening_OnOnSearchUpdate(object? sender, SearchStatistics statistics)
        {
            var totalSeconds = Math.Max(1, statistics.SearchTime) / 1000f;
            var megaLeafsPerSecond = (statistics.Leafs / totalSeconds) / 1_000_000;
            var nanosecondsPerLeaf = (totalSeconds / statistics.Leafs) * 1_000_000_000;

            Console.WriteLine($"  Depth: {statistics.Depth}, Best: {statistics.PrincipalVariation[0]}, Score: {statistics.Score}, Leafs: {statistics.Leafs}, Time: {totalSeconds:F} s, " +
                              $"LPS: {megaLeafsPerSecond:F} ML/s, TPL: {nanosecondsPerLeaf:F} ns");
            Console.WriteLine($"  Branching factor: {statistics.BranchingFactor}, Beta cutoffs: {statistics.BetaCutoffs}, " +
                              $"TTHits: {statistics.TTHits}, TTCollisions: {statistics.TTCollisions}");
        }
    }
}