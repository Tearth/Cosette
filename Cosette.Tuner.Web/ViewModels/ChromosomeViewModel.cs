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
        public double Fitness { get; set; }

        public SelfPlayStatisticsViewModel ReferenceEngineStatistics { get; set; }
        public SelfPlayStatisticsViewModel ExperimentalEngineStatistics { get; set; }

        public List<GeneViewModel> Genes { get; set; }
    }
}
