using System.Runtime.CompilerServices;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Moves.Patterns
{
    public static class FilePatternGenerator
    {
        private static readonly ulong[] Patterns = new ulong[64];

        static FilePatternGenerator()
        {
            for (var i = 0; i < Patterns.Length; i++)
            {
                Patterns[i] = GetPatternForField(i) & ~(1ul << i);
            }
        }

        public static ulong GetPattern(int fieldIndex)
        {
            return Patterns[fieldIndex];
        }

        private static ulong GetPatternForField(int fieldIndex)
        {
            var position = Position.FromFieldIndex(fieldIndex);
            return BoardConstants.AFile >> position.X;
        }
    }
}
