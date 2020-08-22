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
            var boardState = new BoardState();
            boardState.SetDefaultState();

            var result = SimplePerft.Run(boardState, Color.White, depth);
            Assert.Equal(expectedLeafsCount, result.LeafsCount);
        }
    }
}
