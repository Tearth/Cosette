using Cosette.Engine.Ai.Score.PieceSquareTables;
using Cosette.Engine.Board;
using Cosette.Engine.Fen;
using Cosette.Engine.Moves.Magic;
using Cosette.Engine.Perft;
using Xunit;

namespace Cosette.Tests
{
    public class SimplePerftTests
    {
        public SimplePerftTests()
        {
            MagicBitboards.InitWithInternalKeys();
            PieceSquareTablesData.BuildPieceSquareTables();
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 20)]
        [InlineData(2, 400)]
        [InlineData(3, 8902)]
        [InlineData(4, 197281)]
        [InlineData(5, 4865609)]
        [InlineData(6, 119060324)]
        [InlineData(7, 3195901860)]
        public void SimplePerft_DefaultBoard(int depth, ulong expectedLeafsCount)
        {
            var boardState = new BoardState(true);
            boardState.SetDefaultState();

            var result = SimplePerft.Run(boardState, depth);
            Assert.Equal(expectedLeafsCount, result.LeafsCount);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 43)]
        [InlineData(2, 1706)]
        [InlineData(3, 72571)]
        [InlineData(4, 2843426)]
        [InlineData(5, 120955130)]
        public void SimplePerft_MidGameBoard(int depth, ulong expectedLeafsCount)
        {
            var boardState = FenToBoard.Parse("r2qr1k1/p2n1p2/1pb3pp/2ppN1P1/1R1PpP2/BQP1n1PB/P4N1P/1R4K1 w - - 0 21", true);

            var result = SimplePerft.Run(boardState, depth);
            Assert.Equal(expectedLeafsCount, result.LeafsCount);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 20)]
        [InlineData(2, 469)]
        [InlineData(3, 9580)]
        [InlineData(4, 222801)]
        [InlineData(5, 4675662)]
        [InlineData(6, 108146453)]
        public void SimplePerft_EndGameBoard(int depth, ulong expectedLeafsCount)
        {
            var boardState = FenToBoard.Parse("7r/8/2k3P1/1p1p2Kp/1P6/2P5/7r/Q7 w - - 0 1", true);

            var result = SimplePerft.Run(boardState, depth);
            Assert.Equal(expectedLeafsCount, result.LeafsCount);
        }
    }
}
