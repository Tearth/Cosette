using System.Collections.Generic;

namespace Cosette.Engine.Perft.Results
{
    public class DividedPerftResult
    {
        public Dictionary<string, ulong> LeafsCount { get; set; }
        public ulong TotalLeafsCount { get; set; }

        public DividedPerftResult()
        {
            LeafsCount = new Dictionary<string, ulong>();
        }
    }
}
