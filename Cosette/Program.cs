using Cosette.Engine.Ai.Search;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Fen;
using Cosette.Engine.Moves.Magic;
using Cosette.Interactive;

namespace Cosette
{
    public class Program
    {
        private static void Main()
        {
            var test1 = FenToBoard.Parse("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            var test2 = FenToBoard.Parse("r2qr1k1/p2n1p2/1pb3pp/2ppN1P1/1R1PpP2/BQP1n1PB/P4N1P/1R4K1 w - - 0 21");
            var test3 = FenToBoard.Parse("7r/8/2k3P1/1p1p2Kp/1P6/2P5/7r/Q7 w - e4 0 1");

            var test1Result = BoardToFen.Parse(test1);
            var test2Result = BoardToFen.Parse(test2);
            var test3Result = BoardToFen.Parse(test3);

            TranspositionTable.Init(SearchConstants.DefaultHashTableSize);
            PawnHashTable.Init(SearchConstants.DefaultPawnHashTableSize);
            MagicBitboards.InitWithInternalKeys();

            new InteractiveConsole().Run();
        }
    }
}
