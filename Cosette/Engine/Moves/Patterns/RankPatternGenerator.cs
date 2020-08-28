using System.Runtime.CompilerServices;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Moves.Patterns
{
    public static class RankPatternGenerator
    {
        private static readonly ulong[] Patterns = new ulong[64];

        static RankPatternGenerator()
        {
            for (var i = 0; i < Patterns.Length; i++)
            {
                Patterns[i] = GetPatternForField(i) & ~(1ul << i);
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
            var position = Position.FromFieldIndex(fieldIndex);
            return BoardConstants.ARank << (position.Y * 8);
        }
    }
}