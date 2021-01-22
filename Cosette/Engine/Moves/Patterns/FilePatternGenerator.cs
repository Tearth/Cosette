using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Moves.Patterns
{
    public static class FilePatternGenerator
    {
        private static readonly ulong[] _patterns = new ulong[8];

        static FilePatternGenerator()
        {
            for (var i = 0; i < _patterns.Length; i++)
            {
                _patterns[i] = GeneratePatternForField(i);
            }
        }

        public static ulong GetPatternForField(int fieldIndex)
        {
            return _patterns[fieldIndex % 8] & ~(1ul << fieldIndex);
        }

        public static ulong GetPatternForFile(int rank)
        {
            return _patterns[rank % 8];
        }

        private static ulong GeneratePatternForField(int file)
        {
            return BoardConstants.HFile << file;
        }
    }
}
