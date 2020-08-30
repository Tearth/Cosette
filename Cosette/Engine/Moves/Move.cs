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
            return $"{Position.FromFieldIndex(From)}{Position.FromFieldIndex(To)}";
        }
    }
}
