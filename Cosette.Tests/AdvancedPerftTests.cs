using Cosette.Engine.Board;
using Cosette.Engine.Moves.Magic;
using Cosette.Engine.Perft;
using Xunit;

namespace Cosette.Tests
{
    public class AdvancedPerftTests
    {
        public AdvancedPerftTests()
        {
            MagicBitboards.InitWithInternalKeys();
        }

        [Theory]
        [InlineData(0, 1, 0, 0, 0, 0, 0)]
        [InlineData(1, 20, 0, 0, 0, 0, 0)]
        [InlineData(2, 400, 0, 0, 0, 0, 0)]
        [InlineData(3, 8902, 34, 0, 0, 12, 0)]
        [InlineData(4, 197281, 1576, 0, 0, 469, 0)]
        [InlineData(5, 4865609, 82719, 258, 0, 27351, 8)]
        [InlineData(6, 119060324, 2812008, 5248, 0, 809099, 347)]
        [InlineData(7, 3195901860, 108329926, 319617, 883453, 33103848, 10828)]
        public void AdvancedPerft_DefaultBoard(int depth, ulong expectedLeafsCount, ulong expectedCapturesCount, ulong expectedEnPassantsCount,
            ulong expectedCastlesCount, ulong expectedChecksCount, ulong expectedCheckmatesCount)
        {
            var boardState = new BoardState(true);
            boardState.SetDefaultState();

            var result = AdvancedPerft.Run(boardState, depth);
            Assert.Equal(expectedLeafsCount, result.Leafs);
            Assert.Equal(expectedCapturesCount, result.Captures);
            Assert.Equal(expectedEnPassantsCount, result.EnPassants);
            Assert.Equal(expectedCastlesCount, result.Castles);
            Assert.Equal(expectedChecksCount, result.Checks);
            Assert.Equal(expectedCheckmatesCount, result.Checkmates);
        }
    }
}