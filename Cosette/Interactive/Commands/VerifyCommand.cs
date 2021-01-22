using Cosette.Engine.Board;
using Cosette.Engine.Fen;
using Cosette.Engine.Perft;

namespace Cosette.Interactive.Commands
{
    public class VerifyCommand : ICommand
    {
        public string Description { get; }
        private readonly InteractiveConsole _interactiveConsole;

        public VerifyCommand(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;
            Description = "Verification of board states";
        }

        public void Run(params string[] parameters)
        {
            TestOpening();
            TestMidGame();
            TestEndGame();
        }

        private void TestOpening()
        {
            var boardState = new BoardState(true);
            boardState.SetDefaultState();

            Test(boardState, "Opening", 6);
        }

        private void TestMidGame()
        {
            var boardState = FenToBoard.Parse("r2qr1k1/p2n1p2/1pb3pp/2ppN1P1/1R1PpP2/BQP1n1PB/P4N1P/1R4K1 w - - 0 21", true);
            Test(boardState, "Midgame", 5);
        }

        private void TestEndGame()
        {
            var boardState = FenToBoard.Parse("7r/8/2k3P1/1p1p2Kp/1P6/2P5/7r/Q7 w - - 0 1", true);
            Test(boardState, "Endgame", 6);
        }

        private void Test(BoardState boardState, string name, int depth)
        {
            var result = VerificationPerft.Run(boardState, depth);
            var verificationStatus = result.VerificationSuccess ? "ok" : "fail";

            _interactiveConsole.WriteLine($"{name} - Leafs: {result.LeafsCount}, Verificaton: {verificationStatus}");
        }
    }
}