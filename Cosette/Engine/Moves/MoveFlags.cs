using System;

namespace Cosette.Engine.Moves
{
    [Flags]
    public enum MoveFlags : byte
    {
        None = 0,
        Kill = 1,
        Castling = 2,
        EnPassant = 4,
        Promotion = 8,
        KnightPromotion = 16,
        BishopPromotion = 32,
        RookPromotion = 64,
        QueenPromotion = 128
    }
}
