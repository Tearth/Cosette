using System;
using System.Collections.Generic;

namespace Cosette.Tuner.Web.Database.Models
{
    public class GenerationModel
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public TestModel Test { get; set; }
        public DateTime CreationTimeUtc { get; set; }
        public double ElapsedTime { get; set; }
        public int BestFitness { get; set; }
        public List<GenerationGeneModel> BestGenes { get; set; }
    }
}
