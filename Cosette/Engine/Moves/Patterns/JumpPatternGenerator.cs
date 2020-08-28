using System.Runtime.CompilerServices;
using Cosette.Engine.Board;

namespace Cosette.Engine.Moves.Patterns
{
    public static class JumpPatternGenerator
    {
        private static readonly ulong[] Patterns = new ulong[64];

        static JumpPatternGenerator()
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
            return ((field & ~BoardConstants.AFile) << 17) |
                   ((field & ~BoardConstants.HFile) << 15) |
                   ((field & ~BoardConstants.AFile & ~BoardConstants.BFile) << 10) |
                   ((field & ~BoardConstants.GFile & ~BoardConstants.HFile) << 6) |
                   ((field & ~BoardConstants.AFile & ~BoardConstants.BFile) >> 6) |
                   ((field & ~BoardConstants.GFile & ~BoardConstants.HFile) >> 10) |
                   ((field & ~BoardConstants.AFile) >> 15) |
                   ((field & ~BoardConstants.HFile) >> 17);
        }
    }
}
