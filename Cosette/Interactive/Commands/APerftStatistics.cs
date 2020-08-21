﻿namespace Cosette.Interactive.Commands
{
    public class APerftStatistics
    {
        public ulong Nodes { get; set; }
        public ulong Captures { get; set; }
        public ulong EnPassants { get; set; }
        public ulong Castles { get; set; }
        public ulong Checks { get; set; }
        public ulong Checkmates { get; set; }
    }
}
