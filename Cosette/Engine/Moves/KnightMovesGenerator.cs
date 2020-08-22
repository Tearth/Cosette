using System.Runtime.CompilerServices;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Moves
{
    public static class KnightMovesGenerator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetMoves(int fieldIndex)
        {
            return JumpPatternGenerator.GetPattern(fieldIndex);
        }
    }
}
