using Cosette.Engine.Board;
using Cosette.Engine.Board.Operators;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class MobilityEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase) - 
                   Evaluate(board, Color.Black, openingPhase, endingPhase);
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var mobility = KnightOperator.GetMobility(board, color) + 
                           BishopOperator.GetMobility(board, color) +
                           RookOperator.GetMobility(board, color) + 
                           QueenOperator.GetMobility(board, color);

            var mobilityOpeningScore = mobility * EvaluationConstants.Mobility[GamePhase.Opening];
            var mobilityEndingScore = mobility * EvaluationConstants.Mobility[GamePhase.Ending];
            return TaperedEvaluation.AdjustToPhase(mobilityOpeningScore, mobilityEndingScore, openingPhase, endingPhase);
        }
    }
}
