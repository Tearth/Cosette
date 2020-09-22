using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class CastlingEvaluator
    {
        public static int Evaluate(BoardState board, float openingPhase, float endingPhase)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase) - Evaluate(board, Color.Black, openingPhase, endingPhase);
        }

        public static int Evaluate(BoardState board, int color, float openingPhase, float endingPhase)
        {
            var result = 0;
            if (board.CastlingDone[color])
            {
                result = (int)(EvaluationConstants.CastlingDone[(int)GamePhase.Opening] * openingPhase) +
                         (int)(EvaluationConstants.CastlingDone[(int)GamePhase.Ending] * endingPhase);
            }
            else if (color == Color.White && (board.Castling & Castling.WhiteCastling) == 0 ||
                     color == Color.Black && (board.Castling & Castling.BlackCastling) == 0)
            {
                result = (int)(EvaluationConstants.CastlingFailed[(int)GamePhase.Opening] * openingPhase) +
                         (int)(EvaluationConstants.CastlingFailed[(int)GamePhase.Ending] * endingPhase);
            }

            return result;
        }
    }
}
