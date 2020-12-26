using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class BishopEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            var whiteEvaluation = Evaluate(board, Color.White, openingPhase, endingPhase);
            var blackEvaluation = Evaluate(board, Color.Black, openingPhase, endingPhase);
            return whiteEvaluation - blackEvaluation;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var pairOfBishops = 0;
            var bishops = board.Pieces[color][Piece.Bishop];
            
            if (BitOperations.Count(bishops) > 1)
            {
                pairOfBishops = 1;
            }

            var pairOfBishopsOpeningScore = pairOfBishops * EvaluationConstants.PairOfBishops;
            var pairOfBishopsAdjusted = TaperedEvaluation.AdjustToPhase(pairOfBishopsOpeningScore, 0, openingPhase, endingPhase);

            return pairOfBishopsAdjusted;
        }
    }
}