using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Moves.Patterns
{
    public static class FilePatternGenerator
    {
        private static readonly ulong[] _patterns = new ulong[64];

        static FilePatternGenerator()
        {
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
            return BoardConstants.AFile >> position.X;
        }
    }
}
