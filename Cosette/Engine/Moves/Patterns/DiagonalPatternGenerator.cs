using Cosette.Engine.Common;

namespace Cosette.Engine.Moves.Patterns
{
    public static class DiagonalPatternGenerator
    {
        private static readonly ulong[] _patterns = new ulong[64];

        static DiagonalPatternGenerator()
        {
            for (var i = 0; i < _patterns.Length; i++)
            {
                var rightTopPattern = GetPatternForField(i, new Position(-1, 1));
                var leftTopPattern = GetPatternForField(i, new Position(1, 1));
                var rightBottomPattern = GetPatternForField(i, new Position(-1, -1));
                var leftBottomPattern = GetPatternForField(i, new Position(1, -1));

                _patterns[i] = rightTopPattern | leftTopPattern | rightBottomPattern | leftBottomPattern;
            }
        }

        public static ulong GetPattern(int fieldIndex)
        {
            return _patterns[fieldIndex];
        }

        private static ulong GetPatternForField(int fieldIndex, Position shift)
        {
            var attacks = 0ul;
            var currentPosition = Position.FromFieldIndex(fieldIndex);

            currentPosition += shift;
            while (currentPosition.IsValid())
            {
                var positionBitIndex = currentPosition.ToFieldIndex();

                attacks |= 1ul << positionBitIndex;
                currentPosition += shift;
            }

            return attacks;
        }
    }
}
