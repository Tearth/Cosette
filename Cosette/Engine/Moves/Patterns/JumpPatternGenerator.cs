using Cosette.Engine.Board;

namespace Cosette.Engine.Moves.Patterns
{
    public static class JumpPatternGenerator
    {
        private static ulong[] _patterns;

        public static void Init()
        {
            _patterns = new ulong[64];

            ulong field = 1;
            for (var i = 0; i < _patterns.Length; i++)
            {
                _patterns[i] = GetPatternForField(field);
                field <<= 1;
            }
        }

        public static ulong GetPattern(int fieldIndex)
        {
            return _patterns[fieldIndex];
        }

        private static ulong GetPatternForField(ulong field)
        {
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
