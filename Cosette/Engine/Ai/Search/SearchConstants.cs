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

        public static int NullWindowMinimalDepth = 4;
        public static int NullWindowDepthReduction = 3;

        public static int LMRMinimalDepth = 2;
        public static int LMRMovesWithoutReduction = 4;
        public static int LMRPvNodeMaxReduction = 2;
        public static int LMRNonPvNodeMaxReduction = 4;
    }
}
