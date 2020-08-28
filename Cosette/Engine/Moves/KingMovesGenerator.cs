using System.Runtime.CompilerServices;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Moves
{
    public static class KingMovesGenerator
    {
#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ulong GetMoves(int fieldIndex)
        {
            return BoxPatternGenerator.GetPattern(fieldIndex);
        }
    }
}