namespace Cosette.Engine.Ai.Score
{
    public class EvaluationStatistics
    {
        public ulong PHTAddedEntries { get; set; }
        public ulong PHTReplacements { get; set; }
        public ulong PHTHits { get; set; }
        public ulong PHTNonHits { get; set; }
        public float PHTHitsPercent => (float)PHTHits * 100 / (PHTHits + PHTNonHits);

        public ulong EHTAddedEntries { get; set; }
        public ulong EHTReplacements { get; set; }
        public ulong EHTHits { get; set; }
        public ulong EHTNonHits { get; set; }
        public float EHTHitsPercent => (float)EHTHits * 100 / (EHTHits + EHTNonHits);
    }
}
