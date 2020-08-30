using Cosette.Engine.Ai.Score.PieceSquareTables;
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
            var openingPhase = board.GetPhaseRatio();
            var endingPhase = 1 - openingPhase;

            result += EvaluateMaterial(board);
            result += EvaluateCastling(board, Color.White) - EvaluateCastling(board, Color.Black);
            result += EvaluatePosition(board, openingPhase, endingPhase, Color.White) - EvaluatePosition(board, openingPhase, endingPhase, Color.Black);
            result += EvaluatePawnStructure(board, Color.White) - EvaluatePawnStructure(board, Color.Black);

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

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int EvaluatePosition(BoardState board, float openingPhase, float endingPhase, Color color)
        {
            var openingScore = board.Position[(int) color][(int) GamePhase.Opening];
            var endingScore = board.Position[(int)color][(int)GamePhase.Opening];

            return (int)(openingScore * openingPhase + endingScore * endingPhase);
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int EvaluatePawnStructure(BoardState board, Color color)
        {
            // Chains, doubled and isolated
            return 0;
        }
    }
}
