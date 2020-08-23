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

        public override string ToString()
        {
            return $"{Position.FromFieldIndex(From)}{Position.FromFieldIndex(To)}";
        }
    }
}
