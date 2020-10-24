using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class CastlingEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase) - 
                   Evaluate(board, Color.Black, openingPhase, endingPhase);
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            if (board.CastlingDone[color])
            {
                var openingScore = EvaluationConstants.CastlingDone[GamePhase.Opening];
                var endingScore = EvaluationConstants.CastlingDone[GamePhase.Ending];
                return TaperedEvaluation.AdjustToPhase(openingScore, endingScore, openingPhase, endingPhase);
            }

            if (color == Color.White && (board.Castling & Castling.WhiteCastling) == 0 || 
                color == Color.Black && (board.Castling & Castling.BlackCastling) == 0)
            {
                var openingScore = EvaluationConstants.CastlingFailed[GamePhase.Opening];
                var endingScore = EvaluationConstants.CastlingFailed[GamePhase.Ending];
                return TaperedEvaluation.AdjustToPhase(openingScore, endingScore, openingPhase, endingPhase);
            }

            return 0;
        }
    }
}
