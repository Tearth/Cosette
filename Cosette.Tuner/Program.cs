using System;
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
            SettingsLoader.Init("settings.json");

            var selection = new EliteSelection();
            var crossover = new OrderedCrossover();
            var mutation = new ReverseSequenceMutation();
            var fitness = new EvaluationFitness();
            var chromosome = new EvaluationChromosome();
            var population = new Population(5, 10, chromosome);

            var geneticAlgorithm = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            geneticAlgorithm.Termination = new GenerationNumberTermination(100);
            geneticAlgorithm.GenerationRan += GeneticAlgorithm_GenerationRan;

            Console.WriteLine("GA running...");
            geneticAlgorithm.Start();

            Console.WriteLine("Best solution found has {0} fitness.", geneticAlgorithm.BestChromosome.Fitness);
        }

        private static void GeneticAlgorithm_GenerationRan(object sender, EventArgs e)
        {
            
        }
    }
}
