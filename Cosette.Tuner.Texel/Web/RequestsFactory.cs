using System.Collections.Generic;
using Cosette.Tuner.Common.Requests;
using Cosette.Tuner.Texel.Settings;
using GeneticSharp.Domain.Chromosomes;

namespace Cosette.Tuner.Texel.Web
{
    public static class RequestsFactory
    {
        public static ChromosomeDataRequest CreateChromosomeRequest(int testId, double fitness, double elapsedTime, IChromosome chromosome)
        {
            return new ChromosomeDataRequest
            {
                TestId = testId,
                ElapsedTime = elapsedTime,
                Fitness = fitness,
                Genes = CreateGenesRequest(chromosome)
            };
        }

        public static GenerationDataRequest CreateGenerationRequest(int testId, double elapsedTime, IChromosome chromosome)
        {
            return new GenerationDataRequest
            {
                TestId = testId,
                BestFitness = chromosome.Fitness.Value,
                ElapsedTime = elapsedTime,
                BestChromosomeGenes = CreateGenesRequest(chromosome)
            };
        }

        public static List<GeneDataRequest> CreateGenesRequest(IChromosome chromosome)
        {
            var genes = new List<GeneDataRequest>();
            for (var geneIndex = 0; geneIndex < SettingsLoader.Data.Genes.Count; geneIndex++)
            {
                genes.Add(new GeneDataRequest
                {
                    Name = SettingsLoader.Data.Genes[geneIndex].Name,
                    Value = (int)chromosome.GetGene(geneIndex).Value
                });
            }

            return genes;
        }
    }
}
