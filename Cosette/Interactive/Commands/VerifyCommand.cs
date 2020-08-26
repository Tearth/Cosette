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
    public class VerifyCommand : ICommand
    {
        public string Description { get; }

        public VerifyCommand()
        {
            Description = "Verification of board states";
        }

        public void Run(params string[] parameters)
        {
            GC.TryStartNoGCRegion(1024 * 1024 * 16);

            TestOpening();
            TestMidGame();
            TestEndGame();

            GC.EndNoGCRegion();
        }

        private void TestOpening()
        {
            var boardState = new BoardState();
            boardState.SetDefaultState();

            Test(boardState, "Opening", 6);
        }

        private void TestMidGame()
        {
            var boardState = FenParser.Parse("r2qr1k1/p2n1p2/1pb3pp/2ppN1P1/1R1PpP2/BQP1n1PB/P4N1P/1R4K1 w - - 0 21", out _);
            Test(boardState, "Midgame", 5);
        }

        private void TestEndGame()
        {
            var boardState = FenParser.Parse("7r/8/2k3P1/1p1p2Kp/1P6/2P5/7r/Q7 w - - 0 1", out _);
            Test(boardState, "Endgame", 6);
        }

        private void Test(BoardState boardState, string name, int depth)
        {
            var result = VerificationPerft.Run(boardState, Color.White, depth);
            var verificationStatus = result.VerificationSuccess ? "ok" : "fail";

            Console.WriteLine($"{name} - Leafs: {result.LeafsCount}, Verificaton: {verificationStatus}");
        }
    }
}