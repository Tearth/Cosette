using Cosette.Engine.Ai.Score.Evaluators;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score
{
    public class Evaluation
    {
        public static int Evaluate(BoardState board, int color)
        {
            var openingPhase = board.GetPhaseRatio();
            var endingPhase = 1f - openingPhase;

            var result = MaterialEvaluator.Evaluate(board);
            result += CastlingEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += PositionEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += PawnStructureEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += MobilityEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += KingSafetyEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += PiecesEvaluator.Evaluate(board, openingPhase, endingPhase);

            return color == Color.White ? result : -result;
        }
    }
}
