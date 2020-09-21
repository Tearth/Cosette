using System;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Moves
{
    public readonly struct Move
    {
        public readonly byte From;
        public readonly byte To;
        public readonly Piece Piece;
        public readonly MoveFlags Flags;

        public static Move Empty = new Move();

        public Move(byte from, byte to, Piece piece, MoveFlags flags)
        {
            From = from;
            To = to;
            Piece = piece;
            Flags = flags;
        }

        public Move(int from, int to, Piece piece, MoveFlags flags)
        {
            From = (byte)from;
            To = (byte)to;
            Piece = piece;
            Flags = flags;
        }

        public static bool operator ==(Move a, Move b)
        {
            return a.From == b.From && a.To == b.To && a.Piece == b.Piece && a.Flags == b.Flags;
        }

        public static bool operator !=(Move a, Move b)
        {
            return a.From != b.From || a.To != b.To || a.Piece != b.Piece || a.Flags != b.Flags;
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
            return From ^ To ^ (byte)Piece ^ (byte)Flags;
        }

        public static Move FromTextNotation(BoardState board, string textNotation)
        {
            var from = Position.FromText(textNotation.Substring(0, 2));
            var to = Position.FromText(textNotation.Substring(2, 2));
            var flags = textNotation.Length == 5 ? GetMoveFlags(textNotation[4]) : MoveFlags.None;

            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            var movesCount = board.GetAvailableMoves(moves);

            for (var i = 0; i < movesCount; i++)
            {
                if (Position.FromFieldIndex(moves[i].From) == from && Position.FromFieldIndex(moves[i].To) == to)
                {
                    if (flags == MoveFlags.None || (moves[i].Flags & flags) != 0)
                    {
                        return moves[i];
                    }
                }
            }

            return Empty;
        }

        public bool IsQuiet()
        {
            return Flags == MoveFlags.None || Flags == MoveFlags.DoublePush;
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
