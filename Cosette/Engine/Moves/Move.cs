﻿using Cosette.Engine.Common;

namespace Cosette.Engine.Moves
{
    public struct Move
    {
        public byte From { get; set; }
        public byte To { get; set; }
        public Piece Piece { get; set; }
        public MoveFlags Flags { get; set; }

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
    }
}