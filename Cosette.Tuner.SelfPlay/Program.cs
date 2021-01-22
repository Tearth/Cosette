using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Cosette.Tuner.Common.Requests;
using Cosette.Tuner.Common.Services;
using Cosette.Tuner.SelfPlay.Genetics;
using Cosette.Tuner.SelfPlay.Settings;
using Cosette.Tuner.SelfPlay.Web;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

namespace Cosette.Tuner.SelfPlay
{
    public class Program
    {
        private static int _testId;
        private static WebService _webService;
        private static Stopwatch _generationStopwatch;

        public static async Task Main(string[] args)
        {
            Console.WriteLine($"[{DateTime.Now}] Tuner start");
            SettingsLoader.Init("settings.json");

            _webService = new WebService();
            _generationStopwatch = new Stopwatch();

            await _webService.EnableIfAvailable();
            _testId = await _webService.RegisterTest(new RegisterTestRequest
            {
                Type = TestType.SelfPlay
            });

            var selection = new EliteSelection();
            var crossover = new UniformCrossover(0.5f);
            var mutation = new UniformMutation(true);
            var fitness = new EvaluationFitness(_testId, _webService);
            var chromosome = new EvaluationChromosome();
            var population = new Population(SettingsLoader.Data.MinPopulation, SettingsLoader.Data.MaxPopulation, chromosome);

            var geneticAlgorithm = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            geneticAlgorithm.Termination = new GenerationNumberTermination(SettingsLoader.Data.GenerationsCount);
            geneticAlgorithm.GenerationRan += GeneticAlgorithm_GenerationRan;

            _generationStopwatch.Start();
            geneticAlgorithm.Start();

            Console.WriteLine("Best solution found has {0} fitness.", geneticAlgorithm.BestChromosome.Fitness);
            Console.ReadLine();
        }

        private static void GeneticAlgorithm_GenerationRan(object sender, EventArgs e)
        {
            var geneticAlgorithm = (GeneticAlgorithm)sender;
            var genesList = new List<string>();

            for (var geneIndex = 0; geneIndex < SettingsLoader.Data.Genes.Count; geneIndex++)
            {
                var name = SettingsLoader.Data.Genes[geneIndex].Name;
                var value = geneticAlgorithm.BestChromosome.GetGene(geneIndex).ToString();

                genesList.Add($"{name}={value}");
            }

            var generationDataRequest = RequestsFactory.CreateGenerationRequest(_testId, _generationStopwatch.Elapsed.TotalSeconds, geneticAlgorithm.BestChromosome);
            _webService.SendGenerationData(generationDataRequest).GetAwaiter().GetResult();

            Console.WriteLine("======================================");
            Console.WriteLine($"[{DateTime.Now}] Generation done!");
            Console.WriteLine($" - best chromosome: {string.Join(", ", genesList)}");
            Console.WriteLine($" - best fitness: {geneticAlgorithm.BestChromosome.Fitness}");
            Console.WriteLine("======================================");

            _generationStopwatch.Restart();
        }
    }
}
