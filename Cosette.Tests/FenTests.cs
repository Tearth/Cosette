using Cosette.Engine.Board;
using Cosette.Engine.Fen;
using Cosette.Engine.Moves.Magic;
using Cosette.Engine.Perft;
using Xunit;

namespace Cosette.Tests
{
    public class FenTests
    {
        [Theory]
        [InlineData("5r2/2P4p/P5Pp/1b6/P1r1N2N/1pp4K/P7/5k2 w - - 0 10")]
        [InlineData("3K2N1/k7/p1p3qB/3p1R1Q/6P1/1pP4r/P6r/8 w KQ - 20 25")]
        [InlineData("1b6/1kB5/4p3/KB1p1P2/2Pp1NNP/5p2/1p1P4/8 w kq - 40 40")]
        [InlineData("4kb1K/4P3/Rp5p/2P5/8/rr1p1N2/P5P1/2q3n1 w Kk e3 70 1")]
        [InlineData("r5R1/8/2Bp3p/2kq4/2N1p3/2KPP3/2pRP1P1/8 w - h3 0 1")]
        public void DividedPerft_DefaultBoard(string fen)
        {
            var boardFromFen = FenToBoard.Parse(fen, true);
            Assert.Equal(fen, boardFromFen.ToString());
        }
    }
}