namespace Cosette.Engine.Ai.Search
{
    public static class SearchConstants
    {
        public const int MinValue = short.MinValue;
        public const int MaxValue = short.MaxValue;
        public const int NoKingValue = MinValue - 10;
        public const int MaxDepth = 32;
        public const int MaxMovesCount = 218;
        public const float DeadlineFactor = 1.5f;

        public static int IIDMinDepth = 4;
        public static int IIDDepthReduction = 2;

        public static int RazoringMinDepth = 1;
        public static int RazoringMaxDepth = 3;
        public static int RazoringMaxDepthDivider = 7;
        public static int RazoringMargin = 70;
        public static int RazoringMarginMultiplier = 150;

        public static int StaticNullMoveMaxDepth = 3;
        public static int StaticNullMoveMaxDepthDivider = 7;
        public static int StaticNullMoveMargin = 300;
        public static int StaticNullMoveMarginMultiplier = 200;

        public static int NullMoveMinDepth = 2;
        public static int NullMoveDepthReduction = 3;
        public static int NullMoveDepthReductionDivider = 4;

        public static int FutilityPruningMaxDepth = 3;
        public static int FutilityPruningMaxDepthDivisor = 7;
        public static int FutilityPruningMargin = 200;
        public static int FutilityPruningMarginMultiplier = 100;
        public static int QFutilityPruningMargin = 100;

        public static int LMRMinDepth = 2;
        public static int LMRMovesWithoutReduction = 3;
        public static int LMRBaseReduction = 1;
        public static int LMRMoveIndexDivider = 4;
        public static int LMRPvNodeMaxReduction = 2;
        public static int LMRNonPvNodeMaxReduction = 4;
        public static int LMRMaxHistoryValueDivider = 2;

        public static int LMPMaxDepth = 4;
        public static int LMPBasePercentMovesToPrune = 30;
        public static int LMPPercentIncreasePerDepth = 20;
    }
}
