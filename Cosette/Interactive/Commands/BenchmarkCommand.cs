using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Xml.XPath;
using Cosette.Engine.Ai;
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
            Description = "Test performance using a few sample positions";
        }

        public void Run(params string[] parameters)
        {
            GC.TryStartNoGCRegion(1024 * 1024 * 16);

            var openingResult = TestOpening();
            var midGameResult = TestMidGame();
            var endGameResult = TestEndGame();
            var total = openingResult + midGameResult + endGameResult;

            Console.WriteLine($"Total time: {total:F} s");

            GC.EndNoGCRegion();
        }

        private double TestOpening()
        {
            var boardState = new BoardState();
            boardState.SetDefaultState();

            return Test(boardState, "Opening", 6);
        }

        private double TestMidGame()
        {
            var boardState = FenParser.Parse("r2qr1k1/p2n1p2/1pb3pp/2ppN1P1/1R1PpP2/BQP1n1PB/P4N1P/1R4K1 w - - 0 21");
            return Test(boardState, "Midgame", 5);
        }

        private double TestEndGame()
        {
            var boardState = FenParser.Parse("7r/8/2k3P1/1p1p2Kp/1P6/2P5/7r/Q7 w - - 0 1");
            return Test(boardState, "Endgame", 6);
        }

        private double Test(BoardState boardState, string name, int depth)
        {
            var statistics = new SearchStatistics();

            var stopwatch = Stopwatch.StartNew();
            var score = NegaMax.FindBestMove(boardState, Color.White, depth, out Move bestMove, statistics);
            stopwatch.Stop();

            var totalSeconds = stopwatch.Elapsed.TotalSeconds;
            var megaLeafsPerSecond = (statistics.LeafsCount / totalSeconds) / 1_000_000;
            var nanosecondsPerLeaf = (totalSeconds / statistics.LeafsCount) * 1_000_000_000;

            Console.WriteLine($"{name} - Best: {bestMove}, Score: {score}, Leafs: {statistics.LeafsCount}, Time: {totalSeconds:F} s, " +
                              $"LPS: {megaLeafsPerSecond:F} ML/s, TPL: {nanosecondsPerLeaf:F} ns");

            return totalSeconds;
        }
    }
}