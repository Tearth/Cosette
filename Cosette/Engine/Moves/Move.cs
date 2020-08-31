using Cosette.Engine.Common;

namespace Cosette.Engine.Moves
{
    public readonly struct Move
    {
        public readonly byte From;
        public readonly byte To;
        public readonly Piece Piece;
        public readonly MoveFlags Flags;

        public Move(byte from, byte to, Piece piece, MoveFlags flags)
        {
            From = from;
            To = to;
            Piece = piece;
            Flags = flags;
        }

        public Move(int from, int to, Piece piece, MoveFlags flags)
        {
            From = (byte) from;
            To = (byte) to;
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

        public override string ToString()
        {
            var baseMove = $"{Position.FromFieldIndex(From)}{Position.FromFieldIndex(To)}";
            if ((int) Flags >= 16)
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
    }
}
