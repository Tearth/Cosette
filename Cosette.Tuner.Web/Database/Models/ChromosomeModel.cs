using System;
using System.Collections.Generic;

namespace Cosette.Tuner.Web.Database.Models
{
    public class ChromosomeModel
    {
        public int Id { get; set; }

        public int ChromosomeId { get; set; }
        public ChromosomeModel Chromosome { get; set; }

        public DateTime CreationTimeUtc { get; set; }
        public double ElapsedTime { get; set; }
        public int Fitness { get; set; }
        public int ReferenceEngineWins { get; set; }
        public int ExperimentalEngineWins { get; set; }
        public int Draws { get; set; }

        public List<EngineStatisticsModel> EnginesStatistics { get; set; }
    }
}
