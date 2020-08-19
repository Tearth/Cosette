using Cosette.Engine.Common;

namespace Cosette.Engine.Moves.Magic
{
    public static class AttacksGenerator
    {
        public static ulong GetFileRankAttacks(ulong board, int fieldIndex)
        {
            return GetAttacksForDirection(board, fieldIndex, new Position(0, 1)) |
                   GetAttacksForDirection(board, fieldIndex, new Position(0, -1)) |
                   GetAttacksForDirection(board, fieldIndex, new Position(1, 0)) | 
                   GetAttacksForDirection(board, fieldIndex, new Position(-1, 0));
        }

        public static ulong GetDiagonalAttacks(ulong board, int fieldIndex)
        {
            return GetAttacksForDirection(board, fieldIndex, new Position(1, 1)) |
                   GetAttacksForDirection(board, fieldIndex, new Position(1, -1)) |
                   GetAttacksForDirection(board, fieldIndex, new Position(-1, 1)) |
                   GetAttacksForDirection(board, fieldIndex, new Position(-1, -1));
        }

        private static ulong GetAttacksForDirection(ulong board, int fieldIndex, Position shift)
        {
            var current = Position.FromFieldIndex(fieldIndex) + shift;
            ulong attacks = 0;

            while (current.IsValid())
            {
                attacks |= 1ul << current.ToFieldIndex();
                if (((1ul << current.ToFieldIndex()) & board) != 0)
                {
                    break;
                }

                current += shift;
            }

            return attacks;
        }
    }
}
