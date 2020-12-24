namespace Cosette.Tuner.Common.Requests
{
    public class SelfPlayStatisticsDataRequest
    {
        public double AverageTimePerGame { get; set; }
        public double AverageDepth { get; set; }
        public double AverageNodesCount { get; set; }
        public double AverageNodesPerSecond { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
    }
}
