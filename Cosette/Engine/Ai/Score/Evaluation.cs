using Cosette.Engine.Ai.Score.PieceSquareTables;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Board.Operators;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score
{
    public class Evaluation
    {
        private static readonly ulong[] _innerFileMasks;
        private static readonly ulong[] _outerFileMasks;
        private static readonly ulong[] _chainMasks;

        static Evaluation()
        {
            _innerFileMasks = new ulong[8];
            _outerFileMasks = new ulong[8];
            _chainMasks = new ulong[64];

            for (var i = 0; i < 8; i++)
            {
                _innerFileMasks[i] = BoardConstants.AFile >> i;

                if (i - 1 >= 0)
                {
                    _outerFileMasks[i] |= BoardConstants.AFile >> (i - 1);
                }

                if (i + 1 < 8)
                {
                    _outerFileMasks[i] |= BoardConstants.AFile >> (i + 1);
                }
            }

            for (var i = 0; i < 64; i++)
            {
                _chainMasks[i] = DiagonalPatternGenerator.GetPattern(i) & BoxPatternGenerator.GetPattern(i);
            }
        }

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
            result += EvaluateMobility(board, Color.White) - EvaluateMobility(board, Color.Black);

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

            var result = EvaluatePawnStructure(board, Color.White) - EvaluatePawnStructure(board, Color.Black);
            PawnHashTable.Add(board.PawnHash, (short) result);

            return result;
        }

        public static int EvaluatePawnStructure(BoardState board, Color color)
        {
            var doubledPawns = 0;
            var isolatedPawns = 0;
            var chainedPawns = 0;
            var passingPawns = 0;
            var enemyColor = ColorOperations.Invert(color);

            for (var i = 0; i < 8; i++)
            {
                var pawnsOnInnerMask = board.Pieces[(int)color][(int)Piece.Pawn] & _innerFileMasks[i];
                var pawnsOnOuterMask = board.Pieces[(int)color][(int)Piece.Pawn] & _outerFileMasks[i];
                var enemyPawnsOnInnerMask = board.Pieces[(int)enemyColor][(int)Piece.Pawn] & _innerFileMasks[i];

                var pawnsCount = (int)BitOperations.Count(pawnsOnInnerMask);
                if (pawnsCount > 1)
                {
                    doubledPawns += pawnsCount - 1;
                }

                if (pawnsOnInnerMask != 0)
                {
                    if (pawnsOnOuterMask == 0)
                    {
                        isolatedPawns += (int)BitOperations.Count(pawnsOnInnerMask);
                    }

                    if (enemyPawnsOnInnerMask == 0)
                    {
                        passingPawns++;
                    }
                }
            }

            var pieces = board.Pieces[(int) color][(int) Piece.Pawn];
            while (pieces != 0)
            {
                var lsb = BitOperations.GetLsb(pieces);
                pieces = BitOperations.PopLsb(pieces);
                var field = BitOperations.BitScan(lsb);

                var chain = _chainMasks[field] & board.Pieces[(int) color][(int) Piece.Pawn];
                if (chain != 0)
                {
                    chainedPawns += (int) BitOperations.Count(chain);
                }
            }

            return doubledPawns * EvaluationConstants.DoubledPawns + 
                   isolatedPawns * EvaluationConstants.IsolatedPawns +
                   chainedPawns * EvaluationConstants.ChainedPawns +
                   passingPawns * EvaluationConstants.PassingPawns;
        }

        public static int EvaluateMobility(BoardState board, Color color)
        {
            var mobility = KnightOperator.GetMobility(board, color) +
                           BishopOperator.GetMobility(board, color) +
                           RookOperator.GetMobility(board, color) +
                           QueenOperator.GetMobility(board, color);
            return mobility * EvaluationConstants.Mobility;
        }
    }
}
