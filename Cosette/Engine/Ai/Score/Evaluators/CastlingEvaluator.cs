using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class CastlingEvaluator
    {
        public static int Evaluate(BoardState board, float openingPhase, float endingPhase)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase) - 
                   Evaluate(board, Color.Black, openingPhase, endingPhase);
        }

        public static int Evaluate(BoardState board, int color, float openingPhase, float endingPhase)
        {
            if (board.CastlingDone[color])
            {
                return (int)(EvaluationConstants.CastlingDone[GamePhase.Opening] * openingPhase +
                             EvaluationConstants.CastlingDone[GamePhase.Ending] * endingPhase);
            }

            if (color == Color.White && (board.Castling & Castling.WhiteCastling) == 0 || 
                color == Color.Black && (board.Castling & Castling.BlackCastling) == 0)
            {
                return (int)(EvaluationConstants.CastlingFailed[GamePhase.Opening] * openingPhase +
                             EvaluationConstants.CastlingFailed[GamePhase.Ending] * endingPhase);
            }

            return 0;
        }
    }
}
