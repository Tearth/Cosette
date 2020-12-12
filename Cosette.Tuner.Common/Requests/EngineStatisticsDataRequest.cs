namespace Cosette.Tuner.Common.Requests
{
    public class EngineStatisticsDataRequest
    {
        public double AverageTimePerGame { get; set; }
        public double AverageDepth { get; set; }
        public double AverageNodesCount { get; set; }
        public double AverageNodesPerSecond { get; set; }
    }
}
