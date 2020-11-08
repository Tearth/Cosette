using System.Collections.Generic;

namespace Cosette.Tuner.Web.ViewModels
{
    public class MainViewModel
    {
        public TestViewModel LastTest { get; set; }
        public List<TestViewModel> Tests { get; set; }
        public List<GenerationViewModel> AllGenerations { get; set; }
        public List<GenerationViewModel> BestGenerations { get; set; }
        public List<ChromosomeViewModel> AllChromosomes { get; set; }
        public List<ChromosomeViewModel> BestChromosomes { get; set; }

        public string GenerationFitnessChartJson { get; set; }
        public string ChromosomeFitnessChartJson { get; set; }
        public string AverageElapsedTimeChartJson { get; set; }
        public string AverageDepthChartJson { get; set; }
        public string AverageNodesChartJson { get; set; }
    }
}
