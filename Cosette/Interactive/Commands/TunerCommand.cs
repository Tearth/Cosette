using System;
using System.Collections.Generic;
using System.Linq;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Common;
using Cosette.Engine.Fen;
using Cosette.Interactive.Commands.Tuner;
using Cosette.Uci;

namespace Cosette.Interactive.Commands
{
    public class TunerCommand : ICommand
    {
        public string Description { get; }
        private readonly InteractiveConsole _interactiveConsole;

        private EpdLoader _epdLoader;
        private List<EpdPositionData> _positions;

        public TunerCommand(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;
            Description = "Run tuner mode";

            _epdLoader = new EpdLoader();
        }

        public void Run(params string[] parameters)
        {
            switch (parameters[0])
            {
                case "load":
                {
                    var path = string.Join(' ', parameters.Skip(1));
                    LoadEpd(path);
                    break;
                }

                case "evaluate":
                {
                    var scalingFactor = double.Parse(parameters[1]);
                    Evaluate(scalingFactor);
                    break;
                }
            }
        }

        private void LoadEpd(string path)
        {
            _positions = _epdLoader.Load(path);
            _interactiveConsole.WriteLine("Ok");
        }

        private void Evaluate(double scalingFactor)
        {
            var sum = 0.0;
            var evaluationStatistics = new EvaluationStatistics();

            foreach (var position in _positions)
            {
                position.BoardState.ColorToMove = Color.White;
                position.BoardState.RecalculateEvaluationDependentValues();

                var evaluation = Evaluation.Evaluate(position.BoardState, false, evaluationStatistics);
                var sigmoidEvaluation = Sigmoid(evaluation, scalingFactor);
                var desiredEvaluation = GetDesiredEvaluation(position.Result);

                sum += Math.Pow(desiredEvaluation - sigmoidEvaluation, 2);
            }

            var error = sum / _positions.Count;
            _interactiveConsole.WriteLine(error.ToString());
        }

        private double Sigmoid(int evaluation, double scalingFactor)
        {
            return 1.0 / (1 + Math.Pow(10, -scalingFactor * evaluation / 400));
        }

        private double GetDesiredEvaluation(EpdGameResult gameResult)
        {
            switch (gameResult)
            {
                case EpdGameResult.BlackWon: return 0;
                case EpdGameResult.Draw: return 0.5;
                case EpdGameResult.WhiteWon: return 1;
            }

            throw new InvalidOperationException();
        }
    }
}