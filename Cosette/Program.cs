using Cosette.Engine.Moves.Patterns;
using Cosette.Interactive;

namespace Cosette
{
    public class Program
    {
        static void Main(string[] args)
        {
            BoxPatternGenerator.Init();
            DiagonalPatternGenerator.Init();
            JumpPatternGenerator.Init();

            new InteractiveConsole().Run();
        }
    }
}
