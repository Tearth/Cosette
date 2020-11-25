using Cosette.Engine.Ai.Score.Evaluators;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score
{
    public class Evaluation
    {
        public static int Evaluate(BoardState board, EvaluationStatistics statistics)
        {
            var openingPhase = board.GetPhaseRatio();
            var endingPhase = BoardConstants.PhaseResolution - openingPhase;

            var result = MaterialEvaluator.Evaluate(board);
            result += CastlingEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += PositionEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += PawnStructureEvaluator.Evaluate(board, statistics, openingPhase, endingPhase);
            result += MobilityEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += KingSafetyEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += PiecesEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += FianchettoEvaluator.Evaluate(board, openingPhase, endingPhase);

            return board.ColorToMove == Color.White ? result : -result;
        }

        public static int FastEvaluate(BoardState board)
        {
            var result = MaterialEvaluator.Evaluate(board);
            return board.ColorToMove == Color.White ? result : -result;
        }
    }
}
