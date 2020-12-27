using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class PositionEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase) -
                   Evaluate(board, Color.Black, openingPhase, endingPhase);
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var positionOpeningScore = board.Position[color][GamePhase.Opening];
            var positionEndingScore = board.Position[color][GamePhase.Ending];
            return TaperedEvaluation.AdjustToPhase(positionOpeningScore, positionEndingScore, openingPhase, endingPhase);
        }
    }
}
