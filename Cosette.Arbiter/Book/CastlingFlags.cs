using System;

namespace Cosette.Arbiter.Book
{
    [Flags]
    public enum CastlingFlags
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
