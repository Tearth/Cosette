namespace Cosette.Engine.Ai.Search
{
    public static class SearchConstants
    {
        public const int MinValue = short.MinValue;
        public const int MaxValue =  short.MaxValue;
        public const int MaxDepth = 32;
        public const int MaxMovesCount = 128;

        public const int DefaultHashTableSize = 32;
        public const int DefaultPawnHashTableSize = 1;
        public const int DefaultEvaluationHashTableSize = 32;

        public const int NullWindowMinimalDepth = 5;
        public const byte NullWindowDepthReduction = 3;

        public const int LMRMinimalDepth = 2;
        public const int LMRMovesWithoutReduction = 2;
        public const int LMRPvNodeDepthReduction = 1;
        public const int LMRNonPvNodeDepthDivisor = 3;
    }
}
