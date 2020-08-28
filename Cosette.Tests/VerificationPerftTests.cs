using System;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Fen;
using Cosette.Engine.Moves.Magic;
using Cosette.Engine.Perft;
using Xunit;

namespace Cosette.Tests
{
    public class VerificationPerftTests
    {
        public VerificationPerftTests()
        {
            MagicBitboards.InitWithInternalKeys();
        }

        [Fact]
        public void VerificationPerft_DefaultBoard()
        {
            var boardState = new BoardState();
            boardState.SetDefaultState();

            var result = VerificationPerft.Run(boardState, 6);
            Assert.True(result.VerificationSuccess);
        }

        [Fact]
        public void VerificationPerft_MidGameBoard()
        {
            var boardState = FenParser.Parse("r2qr1k1/p2n1p2/1pb3pp/2ppN1P1/1R1PpP2/BQP1n1PB/P4N1P/1R4K1 w - - 0 21", out _);

            var result = VerificationPerft.Run(boardState, 5);
            Assert.True(result.VerificationSuccess);
        }

        [Fact]
        public void VerificationPerft_EndGameBoard()
        {
            var boardState = FenParser.Parse("7r/8/2k3P1/1p1p2Kp/1P6/2P5/7r/Q7 w - - 0 1", out _);

            var result = VerificationPerft.Run(boardState, 6);
            Assert.True(result.VerificationSuccess);
        }
    }
}
