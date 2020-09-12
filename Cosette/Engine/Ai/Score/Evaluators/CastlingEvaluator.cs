using System.Runtime.CompilerServices;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class CastlingEvaluator
    {
#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int Evaluate(BoardState board, float openingPhase, float endingPhase)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase) - Evaluate(board, Color.Black, openingPhase, endingPhase);
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int Evaluate(BoardState board, Color color, float openingPhase, float endingPhase)
        {
            var result = 0;
            if (board.CastlingDone[(int)color])
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
