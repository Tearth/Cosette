using System;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Score.Evaluators;
using Cosette.Engine.Board;
using Cosette.Engine.Fen;

namespace Cosette.Interactive.Commands
{
    public class EvaluateCommand : ICommand
    {
        public string Description { get; }
        private InteractiveConsole _interactiveConsole;

        public EvaluateCommand(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;
            Description = "Evaluate the specified position";
        }

        public void Run(params string[] parameters)
        {
            var fen = string.Join(' ', parameters);
            var boardState = FenToBoard.Parse(fen);
            var evaluationStatistics = new EvaluationStatistics();

            var openingPhase = boardState.GetPhaseRatio();
            var endingPhase = BoardConstants.PhaseResolution - openingPhase;

            var fieldsAttackedByWhite = 0ul;
            var fieldsAttackedByBlack = 0ul;

            var materialEvaluation = MaterialEvaluator.Evaluate(boardState);
            var positionEvaluation = PositionEvaluator.Evaluate(boardState, openingPhase, endingPhase);
            var pawnStructureEvaluation = PawnStructureEvaluator.EvaluateWithoutCache(boardState, evaluationStatistics, openingPhase, endingPhase);
            var mobility = MobilityEvaluator.Evaluate(boardState, openingPhase, endingPhase, ref fieldsAttackedByWhite, ref fieldsAttackedByBlack);
            var kingSafety = KingSafetyEvaluator.Evaluate(boardState, openingPhase, endingPhase, fieldsAttackedByWhite, fieldsAttackedByBlack);
            var rooks = RookEvaluator.Evaluate(boardState, openingPhase, endingPhase);
            var bishops = BishopEvaluator.Evaluate(boardState, openingPhase, endingPhase);
            var fianchetto = FianchettoEvaluator.Evaluate(boardState, openingPhase, endingPhase);

            var total = materialEvaluation + positionEvaluation + pawnStructureEvaluation +
                        mobility + kingSafety + fianchetto;

            _interactiveConsole.WriteLine($"Evaluation for board with hash {boardState.Hash} (phase {openingPhase}, " +
                                          $"{boardState.IrreversibleMovesCount} irreversible moves)");

            _interactiveConsole.WriteLine($" = Material: {materialEvaluation}");
            _interactiveConsole.WriteLine($" = Position: {positionEvaluation}");
            _interactiveConsole.WriteLine($" = Pawns: {pawnStructureEvaluation}");
            _interactiveConsole.WriteLine($" = Mobility: {mobility}");
            _interactiveConsole.WriteLine($" = King safety: {kingSafety}");
            _interactiveConsole.WriteLine($" = Rooks: {rooks}");
            _interactiveConsole.WriteLine($" = Bishops: {bishops}");
            _interactiveConsole.WriteLine($" = Fianchetto: {fianchetto}");
            _interactiveConsole.WriteLine();
            _interactiveConsole.WriteLine($" = Total: {total}");
        }
    }
}