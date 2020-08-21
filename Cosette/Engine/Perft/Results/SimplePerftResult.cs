namespace Cosette.Engine.Perft.Results
{
    public class SimplePerftResult
    {
        public ulong LeafsCount { get; set; }
        public double LeafsPerSecond { get; set; }
        public double Time { get; set; }
        public double TimePerLeaf { get; set; }
    }
}
