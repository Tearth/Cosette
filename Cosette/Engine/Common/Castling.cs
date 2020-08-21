using System;
using System.Collections.Generic;
using System.Text;

namespace Cosette.Engine.Common
{
    [Flags]
    public enum Castling
    {
        None = 0,
        WhiteShort = 1,
        WhiteLong = 2,
        BlackShort = 4,
        BlackLong = 8,
        Everything = WhiteShort | WhiteLong | BlackShort | BlackLong
    }
}
