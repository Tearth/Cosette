namespace Cosette.Engine.Ai.Score
{
    public class EvaluationStatistics
    {
        public ulong PHTEntries { get; set; }
        public ulong PHTCollisions { get; set; }
        public ulong PHTHits { get; set; }
        public ulong PHTNonHits { get; set; }
        public float PHTHitsPercent => (float)PHTHits * 100 / (PHTHits + PHTNonHits);

        public ulong EHTEntries { get; set; }
        public ulong EHTCollisions { get; set; }
        public ulong EHTHits { get; set; }
        public ulong EHTNonHits { get; set; }
        public float EHTHitsPercent => (float)EHTHits * 100 / (EHTHits + EHTNonHits);

        public void Clear()
        {
            PHTEntries = 0;
            PHTCollisions = 0;
            PHTHits = 0;
            PHTNonHits = 0;

            EHTEntries = 0;
            EHTCollisions = 0;
            EHTHits = 0;
            EHTNonHits = 0;
        }
    }
}
