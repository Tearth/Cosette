namespace Cosette.Engine.Ai.Score
{
    public class EvaluationStatistics
    {
        public ulong PHTEntries { get; set; }
        public ulong PHTCollisions { get; set; }
        public ulong PHTHits { get; set; }
        public ulong PHTNonHits { get; set; }
        public float PHTHitsPercent => (float)PHTHits * 100 / (PHTHits + PHTNonHits);

        public void Clear()
        {
            PHTEntries = 0;
            PHTCollisions = 0;
            PHTHits = 0;
        }
    }
}
