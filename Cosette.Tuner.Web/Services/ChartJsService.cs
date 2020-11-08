using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosette.Tuner.Web.Database.Models;
using Cosette.Tuner.Web.ViewModels.ChartJs;

namespace Cosette.Tuner.Web.Services
{
    public class ChartJsService
    {
        public ChartJsData<int> GenerateGenerationFitnessData(List<GenerationModel> generations)
        {
            var labels = Enumerable.Range(0, generations.Count).Select(p => p.ToString()).ToList();
            var values = generations.Select(p => p.BestFitness).ToList();
            var datasets = new List<ChartJsDataset<int>>
            {
                GenerateDataset("Generation fitness", values, "#4dc9f6", "#4dc9f6")
            };
            
            return GenerateData(labels, datasets);
        }

        public ChartJsData<int> GenerateChromosomeFitnessData(List<ChromosomeModel> chromosomes)
        {
            var labels = Enumerable.Range(0, chromosomes.Count).Select(p => p.ToString()).ToList();
            var values = chromosomes.Select(p => p.Fitness).ToList();
            var datasets = new List<ChartJsDataset<int>>
            {
                GenerateDataset("Chromosome fitness", values, "#4dc9f6", "#4dc9f6")
            };

            return GenerateData(labels, datasets);
        }

        public ChartJsData<double> GenerateAverageElapsedTimeData(List<GenerationModel> generations)
        {
            var labels = Enumerable.Range(0, generations.Count).Select(p => p.ToString()).ToList();
            var values = generations.Select(p => p.ElapsedTime).ToList();
            var datasets = new List<ChartJsDataset<double>>
            {
                GenerateDataset("Average elapsed time", values, "#4dc9f6", "#4dc9f6")
            };

            return GenerateData(labels, datasets);
        }

        private ChartJsData<T> GenerateData<T>(List<string> labels, List<ChartJsDataset<T>> datasets)
        {
            return new ChartJsData<T>
            {
                Labels = labels,
                Datasets = datasets
            };
        }

        private ChartJsDataset<T> GenerateDataset<T>(string label, List<T> values, string backgroundColor, string borderColor)
        {
            return new ChartJsDataset<T>
            {
                Label = label,
                Data = values,
                BackgroundColor = backgroundColor,
                BorderColor = borderColor
            };
        }
    }
}
