namespace Cosette.Engine.Ai.Search
{
    public static class SearchConstants
    {
        public const int MinValue = short.MinValue;
        public const int MaxValue = short.MaxValue;
        public const int MaxDepth = 32;
        public const int MaxMovesCount = 218;
        public const float DeadlineFactor = 1.5f;

        public const int NullWindowMinimalDepth = 5;
        public const int NullWindowDepthReduction = 3;

        public const int LMRMinimalDepth = 2;
        public const int LMRMovesWithoutReduction = 2;
        public const int LMRPvNodeDepthReduction = 1;
        public const int LMRNonPvNodeDepthDivisor = 3;
    }
}
