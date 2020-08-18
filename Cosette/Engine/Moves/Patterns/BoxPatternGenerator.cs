using Cosette.Engine.Board;

namespace Cosette.Engine.Moves.Patterns
{
    public static class BoxPatternGenerator
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
