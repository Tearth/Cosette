using System.Diagnostics;
using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Fen;

namespace Cosette.Interactive.Commands
{
    public class BenchmarkCommand : ICommand
    {
        public string Description { get; }
        private readonly InteractiveConsole _interactiveConsole;

        public BenchmarkCommand(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;
            Description = "Test NegaMax performance using a few sample positions";
        }

        public void Run(params string[] parameters)
        {
            HashTableAllocator.Allocate(512);

            var stopwatch = Stopwatch.StartNew();
            TestOpening();
            TestMidGame();
            TestEndGame();
            var total = stopwatch.Elapsed.TotalSeconds;

            _interactiveConsole.WriteLine($"Total time: {total:F} s");
        }

        private void TestOpening()
        {
            var boardState = new BoardState(true);
            boardState.SetDefaultState();

            Test(boardState, "Opening", 12);
        }

        private void TestMidGame()
        {
            var boardState = FenToBoard.Parse("r2qr1k1/p2n1p2/1pb3pp/2ppN1P1/1R1PpP2/BQP1n1PB/P4N1P/1R4K1 w - - 0 21", true);
            Test(boardState, "Midgame", 12);
        }

        private void TestEndGame()
        {
            var boardState = FenToBoard.Parse("7r/8/2k3P1/1p1p2Kp/1P6/2P5/7r/Q7 w - - 0 1", true);
            Test(boardState, "Endgame", 17);
        }

        private void Test(BoardState boardState, string name, int depth)
        {
            _interactiveConsole.WriteLine($" == {name}:");

            TranspositionTable.Clear();
            PawnHashTable.Clear();
            EvaluationHashTable.Clear();
            KillerHeuristic.Clear();
            HistoryHeuristic.Clear();

            var context = new SearchContext(boardState)
            {
                MaxDepth = depth
            };

            IterativeDeepening.OnSearchUpdate += IterativeDeepening_OnSearchUpdate;
            IterativeDeepening.FindBestMove(context);
            IterativeDeepening.OnSearchUpdate -= IterativeDeepening_OnSearchUpdate;

            _interactiveConsole.WriteLine();
        }

        private void IterativeDeepening_OnSearchUpdate(object sender, SearchStatistics statistics)
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

#if DEBUG
            // Beta cutoffs at first move
            _interactiveConsole.WriteLine($"   Beta cutoffs at first move: {statistics.BetaCutoffsAtFirstMove} ({statistics.BetaCutoffsAtFirstMovePercent:F} %), " +
                                          $"Q Beta cutoffs at first move: {statistics.QBetaCutoffsAtFirstMove} ({statistics.QBetaCutoffsAtFirstMovePercent:F} %)");

            // Transposition statistics
            _interactiveConsole.WriteLine($"   TT: " +
                                          $"Added: {statistics.TTAddedEntries}, " +
                                          $"Replacements: {statistics.TTReplacements} ({statistics.TTReplacesPercent:F} %), " +
                                          $"Hits: {statistics.TTHits} ({statistics.TTHitsPercent:F} %), " +
                                          $"Missed: {statistics.TTNonHits}, " +
                                          $"Filled: {TranspositionTable.GetFillLevel():F} %");

            // Pawn hash table statistics
            _interactiveConsole.WriteLine($"   PHT: " +
                                          $"Added: {statistics.EvaluationStatistics.PHTAddedEntries}, " +
                                          $"Replacements: {statistics.EvaluationStatistics.PHTReplacements} ({statistics.EvaluationStatistics.PHTReplacesPercent:F} %), " +
                                          $"Hits: {statistics.EvaluationStatistics.PHTHits} ({statistics.EvaluationStatistics.PHTHitsPercent:F} %), " +
                                          $"Missed: {statistics.EvaluationStatistics.PHTNonHits}, " +
                                          $"Filled: {PawnHashTable.GetFillLevel():F} %");

            // Evaluation hash table statistics
            _interactiveConsole.WriteLine($"   EHT: " +
                                          $"Added: {statistics.EvaluationStatistics.EHTAddedEntries}, " +
                                          $"Replacements: {statistics.EvaluationStatistics.EHTReplacements} ({statistics.EvaluationStatistics.EHTReplacesPercent:F} %), " +
                                          $"Hits: {statistics.EvaluationStatistics.EHTHits} ({statistics.EvaluationStatistics.EHTHitsPercent:F} %), " +
                                          $"Missed: {statistics.EvaluationStatistics.EHTNonHits}, " +
                                          $"Filled: {EvaluationHashTable.GetFillLevel():F} %");

            _interactiveConsole.WriteLine($"   Valid TT moves: {statistics.TTValidMoves}, Invalid TT moves: {statistics.TTInvalidMoves}, " +
                                          $"IID hits: {statistics.IIDHits}, Loud generations: {statistics.LoudMovesGenerated}, " +
                                          $"Quiet generations: {statistics.QuietMovesGenerated}");

            _interactiveConsole.WriteLine($"   Extensions: {statistics.Extensions}, Null-move prunes: {statistics.NullMovePrunes}, " +
                                          $"Static null-move prunes: {statistics.StaticNullMovePrunes}");
#endif


            _interactiveConsole.WriteLine();
        }
    }
}