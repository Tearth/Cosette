namespace Cosette.Engine.Ai.Search
{
    public static class SearchConstants
    {
        public const int MinValue = short.MinValue;
        public const int MaxValue = short.MaxValue;
        public const int MaxDepth = 32;
        public const int MaxMovesCount = 218;
        public const float DeadlineFactor = 1.5f;

        public static int IIDMinDepth = 4;
        public static int IIDDepthReduction = 2;

        public static int StaticNullMoveMaxDepth = 3;
        public static int StaticNullMoveMaxDepthDivider = 7;
        public static int StaticNullMoveMargin = 150;
        public static int StaticNullMoveMarginMultiplier = 100;

        public static int NullMoveMinDepth = 3;
        public static int NullMoveDepthReduction = 3;
        public static int NullMoveDepthReductionDivider = 7;

        public static int FutilityPruningMaxDepth = 3;
        public static int FutilityPruningMaxDepthDivisor = 7;
        public static int FutilityPruningMargin = 200;
        public static int FutilityPruningMarginMultiplier = 250;

        public static int LMRMinDepth = 2;
        public static int LMRMovesWithoutReduction = 4;
        public static int LMRBaseReduction = 1;
        public static int LMRMoveIndexDivider = 4;
        public static int LMRPvNodeMaxReduction = 2;
        public static int LMRNonPvNodeMaxReduction = 4;
    }
}
