using System;
using System.Collections.Generic;

namespace Cosette.Tuner.Web.Database.Models
{
    public class ChromosomeModel
    {
        public int Id { get; set; }

        public int TestId { get; set; }
        public virtual TestModel Test { get; set; }

        public DateTime CreationTimeUtc { get; set; }
        public double ElapsedTime { get; set; }
        public double Fitness { get; set; }

        public virtual List<SelfPlayStatisticsModel> SelfPlayStatistics { get; set; }
        public virtual List<ChromosomeGeneModel> Genes { get; set; }
    }
}
