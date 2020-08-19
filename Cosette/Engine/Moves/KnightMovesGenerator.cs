using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Moves
{
    public static class KnightMovesGenerator
    {
        public static ulong GetMove(int fieldIndex)
        {
            return JumpPatternGenerator.GetPattern(fieldIndex);
        }
    }
}
