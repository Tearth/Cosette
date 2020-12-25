using System;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Score.Evaluators;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Fen;

namespace Cosette.Interactive.Commands
{
    public class EvaluateRawCommand : ICommand
    {
        public string Description { get; }
        private InteractiveConsole _interactiveConsole;

        public EvaluateRawCommand(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;
            Description = "Evaluate the specified position and display only result";
        }

        public void Run(params string[] parameters)
        {
            var fen = string.Join(' ', parameters);
            var boardState = FenToBoard.Parse(fen);
            boardState.ColorToMove = Color.White;

            var evaluationStatistics = new EvaluationStatistics();
            var evaluation = Evaluation.Evaluate(boardState, false, evaluationStatistics);

            _interactiveConsole.WriteLine(evaluation.ToString());
        }
    }
}