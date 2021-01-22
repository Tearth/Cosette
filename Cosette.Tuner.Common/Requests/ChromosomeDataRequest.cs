using System.Collections.Generic;

namespace Cosette.Tuner.Common.Requests
{
    public class ChromosomeDataRequest
    {
        public int TestId { get; set; }
        public double ElapsedTime { get; set; }
        public double Fitness { get; set; }

        public SelfPlayStatisticsDataRequest ReferenceEngineStatistics { get; set; }
        public SelfPlayStatisticsDataRequest ExperimentalEngineStatistics { get; set; }
        public List<GeneDataRequest> Genes { get; set; }
    }
}
