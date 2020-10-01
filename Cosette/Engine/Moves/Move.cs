using System;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Moves
{
    public struct Move
    {
        public byte From => (byte)(_data & 0x3F);
        public byte To => (byte)((_data >> 6) & 0x3F);
        public MoveFlags Flags => (MoveFlags)(_data >> 12);
        public static Move Empty = new Move();

        private ushort _data;

        public Move(byte from, byte to, MoveFlags flags)
        {
            _data = from;
            _data |= (ushort)(to << 6);
            _data |= (ushort)((byte)flags << 12);
        }

        public Move(int from, int to, MoveFlags flags)
        {
            _data = (ushort)from;
            _data |= (ushort)(to << 6);
            _data |= (ushort)((byte)flags << 12);
        }

        public static bool operator ==(Move a, Move b)
        {
            return a._data == b._data;
        }

        public static bool operator !=(Move a, Move b)
        {
            return a._data != b._data;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return this == (Move)obj;
        }

        public override int GetHashCode()
        {
            return From ^ To ^ (byte)Flags;
        }

        public static Move FromTextNotation(BoardState board, string textNotation)
        {
            var from = Position.FromText(textNotation.Substring(0, 2));
            var to = Position.FromText(textNotation.Substring(2, 2));
            var flags = textNotation.Length == 5 ? GetMoveFlags(textNotation[4]) : MoveFlags.Quiet;

            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            var movesCount = board.GetAvailableMoves(moves);

            for (var i = 0; i < movesCount; i++)
            {
                if (Position.FromFieldIndex(moves[i].From) == from && Position.FromFieldIndex(moves[i].To) == to)
                {
                    if (flags == MoveFlags.Quiet || (moves[i].Flags & flags) != 0)
                    {
                        return moves[i];
                    }
                }
            }

            return Empty;
        }

        public bool IsQuiet()
        {
            return Flags == MoveFlags.Quiet || Flags == MoveFlags.DoublePush;
        }

        public override string ToString()
        {
            var baseMove = $"{Position.FromFieldIndex(From)}{Position.FromFieldIndex(To)}";
            if ((int)Flags >= 16)
            {
                return baseMove + GetPromotionSymbol(Flags);
            }
            else
            {
                return baseMove;
            }
        }

        private string GetPromotionSymbol(MoveFlags flags)
        {
            if ((flags & MoveFlags.KnightPromotion) != 0)
            {
                return "n";
            }
            else if ((flags & MoveFlags.BishopPromotion) != 0)
            {
                return "b";
            }
            else if((flags & MoveFlags.RookPromotion) != 0)
            {
                return "r";
            }
            else if((flags & MoveFlags.QueenPromotion) != 0)
            {
                return "q";
            }

            return null;
        }

        private static MoveFlags GetMoveFlags(char promotionPiece)
        {
            switch (promotionPiece)
            {
                case 'q': return MoveFlags.QueenPromotion;
                case 'r': return MoveFlags.RookPromotion;
                case 'b': return MoveFlags.BishopPromotion;
                case 'n': return MoveFlags.KnightPromotion;
            }

            throw new InvalidOperationException();
        }
    }
}
