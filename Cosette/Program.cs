using System.Diagnostics;
using System.Threading;
using Cosette.Engine.Ai;
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
            TranspositionTable.Init(4);
            MagicBitboards.InitWithInternalKeys();

            new InteractiveConsole().Run();
        }
    }
}
