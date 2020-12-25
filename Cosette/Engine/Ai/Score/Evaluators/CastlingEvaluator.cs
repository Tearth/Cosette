using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class CastlingEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            var whiteEvaluation = Evaluate(board, Color.White, openingPhase, endingPhase);
            var blackEvaluation = Evaluate(board, Color.Black, openingPhase, endingPhase);
            return whiteEvaluation - blackEvaluation;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            if (board.CastlingDone[color])
            {
                return TaperedEvaluation.AdjustToPhase(EvaluationConstants.CastlingDone, 0, openingPhase, endingPhase);
            }

            var whiteCastlingFailed = color == Color.White && (board.Castling & Castling.WhiteCastling) == 0;
            var blackCastlingFailed = color == Color.Black && (board.Castling & Castling.BlackCastling) == 0;
            
            if (whiteCastlingFailed || blackCastlingFailed)
            {
                return TaperedEvaluation.AdjustToPhase(EvaluationConstants.CastlingFailed, 0, openingPhase, endingPhase);
            }

            return 0;
        }
    }
}
