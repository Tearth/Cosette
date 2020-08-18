using Cosette.Engine.Board;

namespace Cosette.Engine.Moves.Patterns
{
    public static class JumpPatternGenerator
    {
        private static ulong[] _patterns;

        public static void Init()
        {
            _patterns = new ulong[64];

            for (var i = 0; i < _patterns.Length; i++)
            {
                _patterns[i] = GetPatternForField(i);
            }
        }

        public static ulong GetPattern(int fieldIndex)
        {
            return _patterns[fieldIndex];
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
