using System;

namespace Cosette.Tuner.Web.Database.Models
{
    public class SelfPlayStatisticsModel
    {
        public int Id { get; set; }

        public int ChromosomeId { get; set; }
        public virtual ChromosomeModel Chromosome { get; set; }

        public DateTime CreationTimeUtc { get; set; }
        public bool IsReferenceEngine { get; set; }
        public double AverageTimePerGame { get; set; }
        public double AverageDepth { get; set; }
        public double AverageNodesCount { get; set; }
        public double AverageNodesPerSecond { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
    }
}
