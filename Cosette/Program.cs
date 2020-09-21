using Cosette.Engine.Ai.Search;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Moves.Magic;
using Cosette.Interactive;

namespace Cosette
{
    public class Program
    {
        static void Main(string[] args)
        {
            TranspositionTable.Init(SearchConstants.DefaultHashTableSize);
            PawnHashTable.Init(SearchConstants.DefaultPawnHashTableSize);
            MagicBitboards.InitWithInternalKeys();

            new InteractiveConsole().Run();
        }
    }
}
