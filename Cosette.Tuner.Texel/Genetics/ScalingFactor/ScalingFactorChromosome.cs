using Cosette.Tuner.Texel.Settings;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Randomizations;

namespace Cosette.Tuner.Texel.Genetics.ScalingFactor
{
    public class ScalingFactorChromosome : ChromosomeBase
    {
        public ScalingFactorChromosome() : base(2)
        {
            CreateGenes();
        }

        public override Gene GenerateGene(int geneIndex)
        {
            return new Gene(RandomizationProvider.Current.GetInt(0, 2000));
        }

        public override IChromosome CreateNew()
        {
            return new ScalingFactorChromosome();
        }
    }
}