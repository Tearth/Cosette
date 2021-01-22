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
        private const string ReferenceColor = "#4dc9f6";
        private const string ExperimentalColor = "#ff0000";

        public ChartJsData<double> GenerateGenerationFitnessData(List<GenerationModel> generations)
        {
            var labels = Enumerable.Range(0, generations.Count).Select(p => p.ToString()).ToList();
            var values = generations.Select(p => p.BestFitness).ToList();
            var datasets = new List<ChartJsDataset<double>>
            {
                GenerateDataset("Fitness", values, ReferenceColor)
            };
            
            return GenerateData(labels, datasets);
        }

        public ChartJsData<double> GenerateChromosomeFitnessData(List<ChromosomeModel> chromosomes)
        {
            var labels = Enumerable.Range(0, chromosomes.Count).Select(p => p.ToString()).ToList();
            var values = chromosomes.Select(p => p.Fitness).ToList();
            var datasets = new List<ChartJsDataset<double>>
            {
                GenerateDataset("Fitness", values, ReferenceColor)
            };

            return GenerateData(labels, datasets);
        }

        public ChartJsData<double> GenerateAverageElapsedTimeData(List<GenerationModel> generations)
        {
            var labels = Enumerable.Range(0, generations.Count).Select(p => p.ToString()).ToList();
            var values = generations.Select(p => p.ElapsedTime).ToList();
            var datasets = new List<ChartJsDataset<double>>
            {
                GenerateDataset("Time", values, ReferenceColor)
            };

            return GenerateData(labels, datasets);
        }

        public ChartJsData<double> GenerateAverageDepthData(List<ChromosomeModel> chromosomes)
        {
            var labels = Enumerable.Range(0, chromosomes.Count).Select(p => p.ToString()).ToList();
            var referenceValues = chromosomes
                .SelectMany(p => p.SelfPlayStatistics)
                .Where(p => p.IsReferenceEngine)
                .Select(p => p.AverageDepth)
                .ToList();
            var experimentalValues = chromosomes
                .SelectMany(p => p.SelfPlayStatistics)
                .Where(p => !p.IsReferenceEngine)
                .Select(p => p.AverageDepth)
                .ToList();

            var datasets = new List<ChartJsDataset<double>>
            {
                GenerateDataset("Reference", referenceValues, ReferenceColor),
                GenerateDataset("Experimental", experimentalValues, ExperimentalColor),
            };

            return GenerateData(labels, datasets);
        }

        public ChartJsData<double> GenerateAverageNodesData(List<ChromosomeModel> chromosomes)
        {
            var labels = Enumerable.Range(0, chromosomes.Count).Select(p => p.ToString()).ToList();
            var referenceValues = chromosomes
                .SelectMany(p => p.SelfPlayStatistics)
                .Where(p => p.IsReferenceEngine)
                .Select(p => p.AverageNodesCount)
                .ToList();
            var experimentalValues = chromosomes
                .SelectMany(p => p.SelfPlayStatistics)
                .Where(p => !p.IsReferenceEngine)
                .Select(p => p.AverageNodesCount)
                .ToList();

            var datasets = new List<ChartJsDataset<double>>
            {
                GenerateDataset("Reference", referenceValues, ReferenceColor),
                GenerateDataset("Experimental", experimentalValues, ExperimentalColor),
            };

            return GenerateData(labels, datasets);
        }

        public ChartJsData<double> GenerateAverageTimePerGameData(List<ChromosomeModel> chromosomes)
        {
            var labels = Enumerable.Range(0, chromosomes.Count).Select(p => p.ToString()).ToList();
            var values = chromosomes
                .SelectMany(p => p.SelfPlayStatistics)
                .Where(p => p.IsReferenceEngine)
                .Select(p => p.AverageTimePerGame)
                .ToList();

            var datasets = new List<ChartJsDataset<double>>
            {
                GenerateDataset("Time", values, ReferenceColor)
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

        private ChartJsDataset<T> GenerateDataset<T>(string label, List<T> values, string color)
        {
            return new ChartJsDataset<T>
            {
                Label = label,
                Data = values,
                BorderColor = color,
                BackgroundColor = color,
                Fill = "false"
            };
        }
    }
}
