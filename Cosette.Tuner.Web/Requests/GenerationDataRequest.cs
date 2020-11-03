using System.Collections.Generic;

namespace Cosette.Tuner.Web.Requests
{
    public class GenerationDataRequest
    {
        public string TestId { get; set; }
        public double Time { get; set; }

        public int BestFitness { get; set; }
        public List<GeneDataRequest> BestChromosomeGenes { get; set; }
    }
}
