using System.Runtime.CompilerServices;
using Cosette.Engine.Board;

namespace Cosette.Engine.Moves.Patterns
{
    public static class BoxPatternGenerator
    {
        private static readonly ulong[] Patterns = new ulong[64];

        static BoxPatternGenerator()
        {
            for (var i = 0; i < Patterns.Length; i++)
            {
                Patterns[i] = GetPatternForField(i);
            }
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ulong GetPattern(int fieldIndex)
        {
            return Patterns[fieldIndex];
        }

        private static ulong GetPatternForField(int fieldIndex)
        {
            var field = 1ul << fieldIndex;
            return ((field & ~BoardConstants.AFile) << 1) |
                   ((field & ~BoardConstants.HFile) << 7) |
                   (field << 8) |
                   ((field & ~BoardConstants.AFile) << 9) |
                   ((field & ~BoardConstants.HFile) >> 1) |
                   ((field & ~BoardConstants.AFile) >> 7) |
                   (field >> 8) |
                   ((field & ~BoardConstants.HFile) >> 9);
        }
    }
}
