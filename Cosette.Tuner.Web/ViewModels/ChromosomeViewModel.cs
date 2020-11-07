using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosette.Tuner.Web.ViewModels
{
    public class ChromosomeViewModel
    {
        public int Id { get; set; }

        public DateTime CreationTimeUtc { get; set; }
        public double ElapsedTime { get; set; }
        public int Fitness { get; set; }
        public int ReferenceEngineWins { get; set; }
        public int ExperimentalEngineWins { get; set; }
        public int Draws { get; set; }

        public virtual List<EngineStatisticsViewModel> EnginesStatistics { get; set; }
        public virtual List<GeneViewModel> Genes { get; set; }
    }
}
