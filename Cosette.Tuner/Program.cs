using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cosette.Tuner.Common.Requests;
using Cosette.Tuner.Genetics;
using Cosette.Tuner.Settings;
using Cosette.Tuner.Web;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

namespace Cosette.Tuner
{
    public class Program
    {
        private static WebService _webService;

        public static async Task Main(string[] args)
        {
            Console.WriteLine($"[{DateTime.Now}] Tuner start");
            SettingsLoader.Init("settings.json");

            _webService = new WebService();
            await _webService.EnableIfAvailable();

            var testName = DateTime.Now.Ticks.ToString();
            var selection = new EliteSelection();
            var crossover = new UniformCrossover(0.5f);
            var mutation = new UniformMutation(true);
            var fitness = new EvaluationFitness(testName, _webService);
            var chromosome = new EvaluationChromosome();
            var population = new Population(SettingsLoader.Data.MinPopulation, SettingsLoader.Data.MaxPopulation, chromosome);

            var geneticAlgorithm = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            geneticAlgorithm.Termination = new GenerationNumberTermination(SettingsLoader.Data.GenerationsCount);
            geneticAlgorithm.GenerationRan += GeneticAlgorithm_GenerationRan;
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

            var generationDataRequest = CreateGenerationDataRequest(geneticAlgorithm);
            _webService.SendGenerationData(generationDataRequest).GetAwaiter().GetResult();

            Console.WriteLine("======================================");
            Console.WriteLine($"[{DateTime.Now}] Generation done!");
            Console.WriteLine($" - best chromosome: {string.Join(", ", genesList)}");
            Console.WriteLine($" - best fitness: {geneticAlgorithm.BestChromosome.Fitness}");
            Console.WriteLine("======================================");
        }

        private static GenerationDataRequest CreateGenerationDataRequest(GeneticAlgorithm geneticAlgorithm)
        {
            return new GenerationDataRequest
            {
                TestName = "asd",
                BestFitness = 123,
                ElapsedTime = 12.345,
                BestChromosomeGenes = new List<GeneDataRequest>
                {
                    new GeneDataRequest
                    {
                        Name = "a",
                        Value = 1
                    },
                    new GeneDataRequest
                    {
                        Name =  "b",
                        Value = 2
                    }
                }
            };
        }
    }
}
