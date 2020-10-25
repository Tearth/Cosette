using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Cosette.Tuner.Genetics
{
    public class EvaluationFitness : IFitness
    {
        public double Evaluate(IChromosome chromosome)
        {
            return 1;
        }
    }
}
