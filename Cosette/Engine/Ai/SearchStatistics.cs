﻿namespace Cosette.Engine.Ai
{
    public class SearchStatistics
    {
        public ulong Leafs { get; set; }
        public ulong Nodes { get; set; }
        public ulong SearchTime { get; set; }
        public ulong NodesPerSecond { get; set; }

        public void Clear()
        {
            Leafs = 0;
            Nodes = 0;
            SearchTime = 0;
            NodesPerSecond = 0;
        }
    }
}