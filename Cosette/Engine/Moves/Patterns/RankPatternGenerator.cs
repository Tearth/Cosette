using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Moves.Patterns
{
    public static class RankPatternGenerator
    {
        private static readonly ulong[] _patterns = new ulong[8];

        static RankPatternGenerator()
        {
            for (var i = 0; i < _patterns.Length; i++)
            {
                _patterns[i] = GeneratePatternForField(i);
            }
        }

        public static ulong GetPatternForField(int fieldIndex)
        {
            return _patterns[fieldIndex / 8] & ~(1ul << fieldIndex);
        }

        public static ulong GetPatternForRank(int file)
        {
            return _patterns[file];
        }

        private static ulong GeneratePatternForField(int rank)
        {
            return BoardConstants.ARank << (rank * 8);
        }
    }
}