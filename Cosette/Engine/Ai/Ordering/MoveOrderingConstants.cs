namespace Cosette.Engine.Ai.Ordering
{
    public static class MoveOrderingConstants
    {
        public const short HashMove = 10000;
        public const short Promotion = 5000;
        public const short Capture = 100;
        public const short KillerMove = 80;
        public const short PawnNearPromotion = 50;

        public const int KillerSlots = 3;
    }
}
