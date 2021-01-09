namespace Cosette.Engine.Ai.Search
{
    public static class SearchConstants
    {
        public const int MinValue = short.MinValue;
        public const int MaxValue = short.MaxValue;
        public const int MaxDepth = 32;
        public const int MaxMovesCount = 218;
        public const float DeadlineFactor = 1.5f;

        public static int IIDMinimalDepth = 4;
        public static int IIDDepthReduction = 2;

        public static int StaticNullMoveMaximalDepth = 3;
        public static int StaticNullMoveMaximalDepthDivider = 7;
        public static int StaticNullMoveMargin = 200;
        public static int StaticNullMoveMarginMultiplier = 300;

        public static int NullMoveMinimalDepth = 3;
        public static int NullMoveDepthReduction = 3;
        public static int NullMoveDepthReductionDivider = 7;

        public static int FutilityPruningMaximalDepth = 3;
        public static int FutilityPruningMaximalDepthDivisor = 7;
        public static int FutilityPruningMargin = 200;
        public static int FutilityPruningMarginMultiplier = 300;

        public static int LMRMinimalDepth = 2;
        public static int LMRMovesWithoutReduction = 4;
        public static int LMRBaseReduction = 0;
        public static int LMRMoveIndexDivider = 4;
        public static int LMRPvNodeMaxReduction = 2;
        public static int LMRNonPvNodeMaxReduction = 4;
    }
}
