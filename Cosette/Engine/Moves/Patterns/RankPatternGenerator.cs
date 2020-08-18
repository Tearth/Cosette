using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Moves.Patterns
{
    public static class RankPatternGenerator
    {
        private static ulong[] _patterns;

        public static void Init()
        {
            _patterns = new ulong[64];

            for (var i = 0; i < _patterns.Length; i++)
            {
                _patterns[i] = GetPatternForField(i) & ~(1ul << i);
            }
        }

        public static ulong GetPattern(int fieldIndex)
        {
            return _patterns[fieldIndex];
        }

        private static ulong GetPatternForField(int fieldIndex)
        {
            var position = Position.FromFieldIndex(fieldIndex);
            return BoardConstants.ARank << (position.Y * 8);
        }
    }
}