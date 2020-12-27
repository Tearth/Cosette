using System;
using System.Diagnostics;
using Cosette.Tuner.Common.Services;
using Cosette.Tuner.Texel.Engine;
using Cosette.Tuner.Texel.Settings;
using Cosette.Tuner.Texel.Web;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Cosette.Tuner.Texel.Genetics
{
    public class EvaluationFitness : IFitness
    {
        private int _testId;
        private WebService _webService;

        private EngineOperator _engineOperator;

        public EvaluationFitness(int testId, WebService webService)
        {
            _testId = testId;
            _webService = webService;

            _engineOperator = new EngineOperator(SettingsLoader.Data.EnginePath, SettingsLoader.Data.EngineArguments);
            _engineOperator.Init();
            _engineOperator.LoadEpd(SettingsLoader.Data.PositionsDatabasePath);
        }

        public double Evaluate(IChromosome chromosome)
        {
            for (var geneIndex = 0; geneIndex < SettingsLoader.Data.Genes.Count; geneIndex++)
            {
                var geneName = SettingsLoader.Data.Genes[geneIndex].Name;
                var geneValue = chromosome.GetGene(geneIndex).ToString();

                _engineOperator.SetOption(geneName, geneValue);
            }

            while (true)
            {
                try
                {
                    _engineOperator.ApplyOptions();
                    break;
                }
                catch
                {
                    _engineOperator.Restart();
                    _engineOperator.LoadEpd(SettingsLoader.Data.PositionsDatabasePath);
                }
            }

            var stopwatch = Stopwatch.StartNew();
            var error = _engineOperator.Evaluate();
            var fitness = 1.0 - error;
            var elapsedTime = (double)stopwatch.ElapsedMilliseconds / 1000;

            var chromosomeRequest = RequestsFactory.CreateChromosomeRequest(_testId, fitness, elapsedTime, chromosome);
            _webService.SendChromosomeData(chromosomeRequest).GetAwaiter().GetResult();

            Console.WriteLine($"[{DateTime.Now}] Run done! Fitness: {fitness}");
            return fitness;
        }
    }
}
