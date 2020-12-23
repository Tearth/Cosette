using Cosette.Tuner.SelfPlay.Settings;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace Cosette.Tuner.SelfPlay.Genetics
{
    public class EvaluationChromosome : ChromosomeBase
    {
        public EvaluationChromosome() : base(SettingsLoader.Data.Genes.Count)
        {
            CreateGenes();
        }

        public override Gene GenerateGene(int geneIndex)
        {
            var gene = SettingsLoader.Data.Genes[geneIndex];
            var value = RandomizationProvider.Current.GetInt(gene.MinValue, gene.MaxValue + 1);

            return new Gene(value);
        }

        public override IChromosome CreateNew()
        {
            return new EvaluationChromosome();
        }
    }
}
