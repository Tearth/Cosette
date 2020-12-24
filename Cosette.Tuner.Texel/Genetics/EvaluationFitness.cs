using System;
using System.Diagnostics;
using System.Drawing;
using Cosette.Tuner.Common.Services;
using Cosette.Tuner.Texel.Settings;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Cosette.Tuner.Texel.Genetics
{
    public class EvaluationFitness : IFitness
    {
        private int _testId;
        private WebService _webService;

        public EvaluationFitness(int testId, WebService webService)
        {
            _testId = testId;
            _webService = webService;
        }

        public double Evaluate(IChromosome chromosome)
        {
            return 0;
        }

        private int CalculateEloPerformance(int wins, int losses, int draws)
        {
            return 400 * (wins - losses) / (wins + losses + draws);
        }
    }
}
