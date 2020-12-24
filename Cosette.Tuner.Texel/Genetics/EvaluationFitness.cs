using System;
using Cosette.Tuner.Common.Services;
using Cosette.Tuner.Texel.Engine;
using Cosette.Tuner.Texel.Genetics.Epd;
using Cosette.Tuner.Texel.Settings;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Cosette.Tuner.Texel.Genetics
{
    public class EvaluationFitness : IFitness
    {
        private int _testId;
        private EpdLoader _epdLoader;
        private WebService _webService;

        private EngineOperator _engineOperator;

        public EvaluationFitness(int testId, EpdLoader epdLoader, WebService webService)
        {
            _testId = testId;
            _epdLoader = epdLoader;
            _webService = webService;

            _engineOperator = new EngineOperator(SettingsLoader.Data.EnginePath, SettingsLoader.Data.EngineArguments);
            _engineOperator.Init();
        }

        public double Evaluate(IChromosome chromosome)
        {
            var sum = 0.0;

            foreach (var position in _epdLoader.Positions)
            {
                var evaluation = _engineOperator.Evaluate(position.Fen);
                var sigmoidEvaluation = Sigmoid(evaluation);
                var desiredEvaluation = GetDesiredEvaluation(position.Result);

                sum += Math.Pow(desiredEvaluation - sigmoidEvaluation, 2);
            }

            return sum / _epdLoader.Positions.Count;
        }

        private double Sigmoid(int evaluation)
        {
            return 1.0 / (1 + Math.Pow(10, -SettingsLoader.Data.ScalingConstant * evaluation / 400));
        }

        private double GetDesiredEvaluation(GameResult gameResult)
        {
            switch (gameResult)
            {
                case GameResult.BlackWon: return 0;
                case GameResult.Draw: return 0.5;
                case GameResult.WhiteWon: return 1;
            }

            throw new InvalidOperationException();
        }
    }
}
