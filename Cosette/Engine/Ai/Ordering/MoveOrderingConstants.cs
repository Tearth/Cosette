namespace Cosette.Engine.Ai.Ordering
{
    public static class MoveOrderingConstants
    {
        public static short HashMove = 10000;
        public static short Promotion = 5000;
        public static short Castling = 1000;
        public static short PawnNearPromotion = 150;
        public static short Capture = 100;
        public static short EnPassant = 100;
        public static short KillerMove = 90;
        public static uint HistoryHeuristicMaxScore = 80;

        public static int KillerSlots = 3;
    }
}
