namespace Cosette.Engine.Moves
{
    public enum MoveFlags : byte
    {
        Quiet = 0,
        DoublePush = 1,
        KingCastle = 2,
        QueenCastle = 3,
        Capture = 4,
        EnPassant = 5,
        KnightPromotion = 8,
        BishopPromotion = 9,
        RookPromotion = 10,
        QueenPromotion = 11,
        KnightPromotionCapture = 12,
        BishopPromotionCapture = 13,
        RookPromotionCapture = 14,
        QueenPromotionCapture = 15
    }

    public static class MoveFlagFields
    {
        public const int Special0 = 1;
        public const int Special1 = 2;
        public const int Capture = 4;
        public const int Promotion = 8;
    }
}
