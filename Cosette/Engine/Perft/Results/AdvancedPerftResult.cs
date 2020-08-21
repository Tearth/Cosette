namespace Cosette.Engine.Perft.Results
{
    public class AdvancedPerftResult
    {
        public ulong Leafs { get; set; }
        public ulong Captures { get; set; }
        public ulong EnPassants { get; set; }
        public ulong Castles { get; set; }
        public ulong Checks { get; set; }
        public ulong Checkmates { get; set; }
        public double Time { get; set; }
    }
}
