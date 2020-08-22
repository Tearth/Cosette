using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Xml.XPath;
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
            var total = openingResult.Time + midGameResult.Time + endGameResult.Time;

            Console.WriteLine($"Total time: {total:F} s");

            GC.EndNoGCRegion();
        }

        private SimplePerftResult TestOpening()
        {
            var boardState = new BoardState();
            boardState.SetDefaultState();

            return Test(boardState, "Opening", 6);
        }

        private SimplePerftResult TestMidGame()
        {
            var boardState = FenParser.Parse("r2qr1k1/p2n1p2/1pb3pp/2ppN1P1/1R1PpP2/BQP1n1PB/P4N1P/1R4K1 w - - 0 21");
            return Test(boardState, "Midgame", 5);
        }

        private SimplePerftResult TestEndGame()
        {
            var boardState = FenParser.Parse("7r/8/2k3P1/1p1p2Kp/1P6/2P5/7r/Q7 w - - 0 1");
            return Test(boardState, "Endgame", 6);
        }

        private SimplePerftResult Test(BoardState boardState, string name, int depth)
        {
            var result = SimplePerft.Run(boardState, Color.White, depth);
            var megaLeafsPerSecond = result.LeafsPerSecond / 1_000_000;
            var nanosecondsPerLeaf = result.TimePerLeaf * 1_000_000_000;

            Console.WriteLine($"{name} - Leafs: {result.LeafsCount}, Time: {result.Time:F} s, " +
                              $"LPS: {megaLeafsPerSecond:F} ML/s, TPL: {nanosecondsPerLeaf:F} ns");

            return result;
        }
    }
}