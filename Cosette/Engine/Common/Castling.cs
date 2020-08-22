using System;
using System.Collections.Generic;
using System.Text;

namespace Cosette.Engine.Common
{
    [Flags]
    public enum Castling : byte
    {
        None = 0,
        WhiteShort = 1,
        WhiteLong = 2,
        BlackShort = 4,
        BlackLong = 8,
        WhiteCastling = WhiteShort | WhiteLong,
        BlackCastling = BlackShort | BlackLong,
        Everything = WhiteShort | WhiteLong | BlackShort | BlackLong
    }
}
