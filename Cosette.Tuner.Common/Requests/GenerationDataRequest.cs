using System.Collections.Generic;

namespace Cosette.Tuner.Common.Requests
{
    public class GenerationDataRequest
    {
        public string TestName { get; set; }
        public double ElapsedTime { get; set; }
        public int BestFitness { get; set; }

        public List<GeneDataRequest> BestChromosomeGenes { get; set; }
    }
}
