using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Moves.Patterns
{
    public static class ForwardBoxPatternGenerator
    {
        private static readonly ulong[][] _patterns = new ulong[2][];

        static ForwardBoxPatternGenerator()
        {
            for (var color = 0; color < 2; color++)
            {
                _patterns[color] = new ulong[64];

                for (var fieldIndex = 0; fieldIndex < 64; fieldIndex++)
                {
                    _patterns[color][fieldIndex] = GetPatternForField(color, fieldIndex);
                }
            }
        }

        public static ulong GetPattern(int color, int fieldIndex)
        {
            return _patterns[color][fieldIndex];
        }

        private static ulong GetPatternForField(int color, int fieldIndex)
        {
            var offset = color == Color.White ? 8 : -8;
            var mask = BoxPatternGenerator.GetPattern(fieldIndex);
            var fieldIndexWithOffset = fieldIndex + offset;

            if (fieldIndexWithOffset >= 0 && fieldIndexWithOffset < 64)
            {
                mask |= BoxPatternGenerator.GetPattern(fieldIndexWithOffset);
                mask &= ~(1ul << fieldIndex);
            }

            return mask;
        }
    }
}