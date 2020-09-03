using Cosette.Engine.Ai.Score.PieceSquareTables;
using Cosette.Engine.Ai.Transposition;
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
            result += EvaluatePawnStructure(board);

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
            var openingScore = board.Position[(int) color][(int)GamePhase.Opening];
            var endingScore = board.Position[(int)color][(int)GamePhase.Ending];

            return (int)(openingScore * openingPhase + endingScore * endingPhase);
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int EvaluatePawnStructure(BoardState board)
        {
            var entry = PawnHashTable.Get(board.PawnHash);
            if (entry.Hash == board.PawnHash)
            {
                return entry.Score;
            }

            var doubledPawnsScore = (EvaluateDoubledPawns(board, Color.White) - EvaluateDoubledPawns(board, Color.Black)) * EvaluationConstants.DoubledPawns;
            var isolatedPawnsScore = (EvaluateIsolatedPawns(board, Color.White) - EvaluateIsolatedPawns(board, Color.Black)) * EvaluationConstants.IsolatedPawns;
            var result = doubledPawnsScore + isolatedPawnsScore;

            PawnHashTable.Add(board.PawnHash, (short) result);
            return result;
        }

        private static int EvaluateDoubledPawns(BoardState board, Color color)
        {
            var doubledPawns = 0;
            var mask = BoardConstants.AFile;

            for (var i = 0; i < 8; i++)
            {
                var pawnsOnFile = board.Pieces[(int) color][(int) Piece.Pawn] & mask;
                var pawnsCount = (int) BitOperations.Count(pawnsOnFile);

                if (pawnsCount > 1)
                {
                    doubledPawns += pawnsCount - 1;
                }

                mask >>= 1;
            }

            return doubledPawns;
        }

        private static int EvaluateIsolatedPawns(BoardState board, Color color)
        {
            var isolatedPawns = 0;
            var innerMask = BoardConstants.BFile;
            var outerMask = BoardConstants.AFile | BoardConstants.CFile;

            for (var i = 1; i < 6; i++)
            {
                var pawnsOnInnerMask = board.Pieces[(int)color][(int)Piece.Pawn] & innerMask;
                var pawnsOnOuterMask = board.Pieces[(int)color][(int)Piece.Pawn] & outerMask;

                if (pawnsOnInnerMask != 0 && pawnsOnOuterMask == 0)
                {
                    isolatedPawns += (int) BitOperations.Count(pawnsOnInnerMask);
                }

                innerMask >>= 1;
                outerMask >>= 1;
            }

            return isolatedPawns;
        }
    }
}
