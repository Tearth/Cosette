using System.Diagnostics;
using System.Threading;
using Cosette.Engine.Ai;
using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Score.PieceSquareTables;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Moves.Magic;
using Cosette.Interactive;

namespace Cosette
{
    public class Program
    {
        static void Main(string[] args)
        {
            TranspositionTable.Init(8);
            PawnHashTable.Init(8);

            StaticExchangeEvaluation.Init();
            MagicBitboards.InitWithInternalKeys();

            new InteractiveConsole().Run();
        }
    }
}
