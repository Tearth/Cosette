using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Moves.Patterns
{
    public static class OuterFilesPatternGenerator
    {
        private static readonly ulong[] _patterns = new ulong[8];

        static OuterFilesPatternGenerator()
        {
            for (var i = 0; i < _patterns.Length; i++)
            {
                _patterns[i] = GeneratePatternForField(i);
            }
        }

        public static ulong GetPatternForFile(int file)
        {
            return _patterns[file];
        }

        private static ulong GeneratePatternForField(int fieldIndex)
        {
            var result = 0ul;
            if (fieldIndex + 1 < 8)
            {
                result |= BoardConstants.HFile << (fieldIndex + 1);
            }

            if (fieldIndex - 1 >= 0)
            {
                result |= BoardConstants.HFile << (fieldIndex - 1);
            }

            return result;
        }
    }
}