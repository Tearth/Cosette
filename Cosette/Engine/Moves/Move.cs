using System;
using System.Runtime.CompilerServices;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Moves
{
    public readonly struct Move
    {
        public byte From => (byte)(_data & 0x3F);
        public byte To => (byte)((_data >> 6) & 0x3F);
        public MoveFlags Flags => (MoveFlags)(_data >> 12);
        public static Move Empty = new Move();

        private readonly ushort _data;

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
            return _data;
        }

        public static Move FromTextNotation(BoardState board, string textNotation)
        {
            var from = Position.FromText(textNotation.Substring(0, 2));
            var to = Position.FromText(textNotation.Substring(2, 2));
            var flags = textNotation.Length == 5 ? GetMoveFlags(textNotation[4]) : MoveFlags.Quiet;

            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            var movesCount = board.GetAllMoves(moves);

            for (var i = 0; i < movesCount; i++)
            {
                if (Position.FromFieldIndex(moves[i].From) == from && Position.FromFieldIndex(moves[i].To) == to)
                {
                    if (flags == MoveFlags.Quiet || ((byte)moves[i].Flags & ~MoveFlagFields.Capture) == (byte)flags)
                    {
                        return moves[i];
                    }
                }
            }

            return Empty;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsQuiet()
        {
            return Flags == MoveFlags.Quiet || Flags == MoveFlags.DoublePush;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsSinglePush()
        {
            return Flags == MoveFlags.Quiet;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsDoublePush()
        {
            return Flags == MoveFlags.DoublePush;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnPassant()
        {
            return Flags == MoveFlags.EnPassant;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCapture()
        {
            return ((int)Flags & MoveFlagFields.Capture) != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCastling()
        {
            return Flags == MoveFlags.KingCastle || Flags == MoveFlags.QueenCastle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsKingCastling()
        {
            return Flags == MoveFlags.KingCastle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsQueenCastling()
        {
            return Flags == MoveFlags.QueenCastle;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPromotion()
        {
            return ((int)Flags & MoveFlagFields.Promotion) != 0;
        }

        public override string ToString()
        {
            var baseMove = $"{Position.FromFieldIndex(From)}{Position.FromFieldIndex(To)}";
            if (IsPromotion())
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
            switch (flags)
            {
                case MoveFlags.QueenPromotion:
                case MoveFlags.QueenPromotionCapture:
                {
                    return "q";
                }

                case MoveFlags.RookPromotion:
                case MoveFlags.RookPromotionCapture:
                {
                    return "r";
                }

                case MoveFlags.BishopPromotion:
                case MoveFlags.BishopPromotionCapture:
                {
                    return "b";
                }

                case MoveFlags.KnightPromotion:
                case MoveFlags.KnightPromotionCapture:
                {
                    return "n";
                }
            }

            throw new InvalidOperationException();
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
