using System;
using System.Collections.Generic;

namespace Cosette.Tuner.Web.Database.Models
{
    public class GenerationModel
    {
        public int Id { get; set; }

        public int TestId { get; set; }
        public virtual TestModel Test { get; set; }

        public DateTime CreationTimeUtc { get; set; }
        public double ElapsedTime { get; set; }
        public double BestFitness { get; set; }

        public virtual List<GenerationGeneModel> BestGenes { get; set; }
    }
}
