using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score
{
    public class Evaluation
    {
#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int Evaluate(BoardState board, Color color)
        {
            var result = 0;

            result += EvaluateMaterial(board);
            result += EvaluateCastling(board, color);

            var sign = color == Color.White ? 1 : -1;
            return sign * result;
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int EvaluateMaterial(BoardState board)
        {
            return board.Material[(int)Color.White] - board.Material[(int)Color.Black];
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int EvaluateCastling(BoardState board, Color color)
        {
            var result = 0;
            if (board.CastlingDone[(int) color])
            {
                result += EvaluationConstants.CastlingDone;
            }
            else
            {
                if (color == Color.White && (board.Castling & Castling.WhiteCastling) == 0 ||
                    color == Color.Black && (board.Castling & Castling.BlackCastling) == 0)
                {
                    result += EvaluationConstants.CastlingFailed;
                }
            }

            return result;
        }
    }
}
