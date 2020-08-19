using Cosette.Engine.Moves;
using Cosette.Engine.Moves.Magic;
using Cosette.Interactive;

namespace Cosette
{
    public class Program
    {
        static void Main(string[] args)
        {
            MagicBitboards.InitWithInternalKeys();
            new InteractiveConsole().Run();
        }
    }
}
