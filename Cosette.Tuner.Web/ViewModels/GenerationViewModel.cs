using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosette.Tuner.Web.ViewModels
{
    public class GenerationViewModel
    {
        public int Id { get; set; }

        public DateTime CreationTimeUtc { get; set; }
        public double ElapsedTime { get; set; }
        public int BestFitness { get; set; }

        public List<GeneViewModel> BestGenes { get; set; }
    }
}
