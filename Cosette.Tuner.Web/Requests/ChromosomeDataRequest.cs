using System.Collections.Generic;

namespace Cosette.Tuner.Web.Requests
{
    public class ChromosomeDataRequest
    {
        public string TestId { get; set; }
        public double ElapsedTime { get; set; }
        public int Fitness { get; set; }

        public int ReferenceEngineWins { get; set; }
        public int ExperimentalEngineWins { get; set; }
        public int Draws { get; set; }
        public int Errors { get; set; }

        public EngineStatisticsDataRequest ReferenceEngineStatistics { get; set; }
        public EngineStatisticsDataRequest ExperimentalEngineStatistics { get; set; }

        public List<GeneDataRequest> Genes { get; set; }
    }
}
