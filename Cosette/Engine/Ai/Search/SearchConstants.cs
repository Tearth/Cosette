namespace Cosette.Engine.Ai.Search
{
    public static class SearchConstants
    {
        public const int MinValue = -10000000;
        public const int MaxValue =  10000000;
        public const int MaxDepth = 32;

        public const int NullWindowMinimalDepth = 4;
        public const int NullWindowDepthReduction = 3;
        public const int LMRMinimalDepth = 2;
        public const int LMRMovesWithoutReduction = 2;
        public const int LMRDepthReduction = 1;
    }
}
