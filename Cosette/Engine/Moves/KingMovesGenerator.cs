using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Moves
{
    public static class KingMovesGenerator
    {
        public static ulong GetMoves(int fieldIndex)
        {
            return BoxPatternGenerator.GetPattern(fieldIndex);
        }
    }
}