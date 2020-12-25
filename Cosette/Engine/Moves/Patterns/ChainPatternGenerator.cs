using Cosette.Engine.Board;

namespace Cosette.Engine.Moves.Patterns
{
    public static class ChainPatternGenerator
    {
        private static readonly ulong[] _patterns = new ulong[64];

        static ChainPatternGenerator()
        {
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
            return DiagonalPatternGenerator.GetPattern(fieldIndex) & BoxPatternGenerator.GetPattern(fieldIndex);
        }
    }
}