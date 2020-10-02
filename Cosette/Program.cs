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
            HashTableAllocator.Allocate();
            MagicBitboards.InitWithInternalKeys();

            new InteractiveConsole().Run();
        }
    }
}
