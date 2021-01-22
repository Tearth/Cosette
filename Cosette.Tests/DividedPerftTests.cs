using Cosette.Engine.Ai.Score.PieceSquareTables;
using Cosette.Engine.Board;
using Cosette.Engine.Moves.Magic;
using Cosette.Engine.Perft;
using Xunit;

namespace Cosette.Tests
{
    public class DividedPerftTests
    {
        public DividedPerftTests()
        {
            MagicBitboards.InitWithInternalKeys();
            PieceSquareTablesData.BuildPieceSquareTables();
        }

        [Fact]
        public void DividedPerft_DefaultBoard()
        {
            var boardState = new BoardState(true);
            boardState.SetDefaultState();

            var result = DividedPerft.Run(boardState, 6);
            Assert.Equal(4463267ul, result.LeafsCount["a2a3"]);
            Assert.Equal(5310358ul, result.LeafsCount["b2b3"]);
            Assert.Equal(5417640ul, result.LeafsCount["c2c3"]);
            Assert.Equal(8073082ul, result.LeafsCount["d2d3"]);
            Assert.Equal(9726018ul, result.LeafsCount["e2e3"]);
            Assert.Equal(4404141ul, result.LeafsCount["f2f3"]);
            Assert.Equal(5346260ul, result.LeafsCount["g2g3"]);
            Assert.Equal(4463070ul, result.LeafsCount["h2h3"]);
            Assert.Equal(5363555ul, result.LeafsCount["a2a4"]);
            Assert.Equal(5293555ul, result.LeafsCount["b2b4"]);
            Assert.Equal(5866666ul, result.LeafsCount["c2c4"]);
            Assert.Equal(8879566ul, result.LeafsCount["d2d4"]);
            Assert.Equal(9771632ul, result.LeafsCount["e2e4"]);
            Assert.Equal(4890429ul, result.LeafsCount["f2f4"]);
            Assert.Equal(5239875ul, result.LeafsCount["g2g4"]);
            Assert.Equal(5385554ul, result.LeafsCount["h2h4"]);
            Assert.Equal(4856835ul, result.LeafsCount["b1a3"]);
            Assert.Equal(5708064ul, result.LeafsCount["b1c3"]);
            Assert.Equal(5723523ul, result.LeafsCount["g1f3"]);
            Assert.Equal(4877234ul, result.LeafsCount["g1h3"]);
            Assert.Equal(119060324ul, result.TotalLeafsCount);
        }
    }
}