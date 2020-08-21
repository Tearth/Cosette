using System;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
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
        }

        [Theory]
        [InlineData(1, 1)]
        [InlineData(2, 20)]
        [InlineData(3, 400)]
        [InlineData(4, 8902)]
        [InlineData(5, 197281)]
        [InlineData(6, 4865609)]
        [InlineData(7, 119060324)]
        [InlineData(8, 3195901860)]
        public void SimplePerft_DefaultBoard(int depth, ulong expectedLeafsCount)
        {
            var boardState = new BoardState();
            boardState.SetDefaultState();

            var result = SimplePerft.Run(boardState, Color.White, depth);
            Assert.Equal(expectedLeafsCount, result.LeafsCount);
        }
    }
}
