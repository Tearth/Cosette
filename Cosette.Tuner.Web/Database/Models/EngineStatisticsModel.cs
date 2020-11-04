using System;

namespace Cosette.Tuner.Web.Database.Models
{
    public class EngineStatisticsModel
    {
        public int Id { get; set; }

        public int ChromosomeId { get; set; }
        public ChromosomeModel Chromosome { get; set; }

        public DateTime CreationTimeUtc { get; set; }
        public bool IsReferenceEngine { get; set; }
        public double AverageTimePerGame { get; set; }
        public double AverageDepth { get; set; }
        public double AverageNodesCount { get; set; }
        public double AverageNodesPerSecond { get; set; }
    }
}
