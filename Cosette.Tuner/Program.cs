using System;
using System.Collections.Generic;
using Cosette.Tuner.Genetics;
using Cosette.Tuner.Settings;
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
        public static void Main(string[] args)
        {
            Console.WriteLine($"[{DateTime.Now}] Tuner start");
            SettingsLoader.Init("settings.json");

            var selection = new EliteSelection();
            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();
            var fitness = new EvaluationFitness();
            var chromosome = new EvaluationChromosome();
            var population = new Population(10, 10, chromosome);

            var geneticAlgorithm = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            geneticAlgorithm.Termination = new GenerationNumberTermination(100);
            geneticAlgorithm.GenerationRan += GeneticAlgorithm_GenerationRan;
            geneticAlgorithm.Start();

            Console.WriteLine("Best solution found has {0} fitness.", geneticAlgorithm.BestChromosome.Fitness);
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


            Console.WriteLine("======================================");
            Console.WriteLine($"[{DateTime.Now}] Generation done!");
            Console.WriteLine($" - best chromosome: {string.Join(", ", genesList)}");
            Console.WriteLine($" - best fitness: {geneticAlgorithm.BestChromosome.Fitness}");
            Console.WriteLine("======================================");
        }
    }
}
