using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Moves
{
    public static class KingMovesGenerator
    {
        public static ulong GetMove(int fieldIndex)
        {
            return BoxPatternGenerator.GetPattern(fieldIndex);
        }
    }
}