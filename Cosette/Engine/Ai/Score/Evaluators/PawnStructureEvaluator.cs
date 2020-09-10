using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class PawnStructureEvaluator
    {
        private static readonly ulong[] _innerFileMasks;
        private static readonly ulong[] _outerFileMasks;
        private static readonly ulong[] _chainMasks;

        static PawnStructureEvaluator()
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
        public static int Evaluate(BoardState board, float openingPhase, float endingPhase)
        {
            var entry = PawnHashTable.Get(board.PawnHash);
            if (entry.Hash == board.PawnHash)
            {
                return entry.Score;
            }

            var result = Evaluate(board, Color.White, openingPhase, endingPhase) - Evaluate(board, Color.Black, openingPhase, endingPhase);
            PawnHashTable.Add(board.PawnHash, (short)result);

            return result;
        }

        public static int Evaluate(BoardState board, Color color, float openingPhase, float endingPhase)
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

            var pieces = board.Pieces[(int)color][(int)Piece.Pawn];
            while (pieces != 0)
            {
                var lsb = BitOperations.GetLsb(pieces);
                pieces = BitOperations.PopLsb(pieces);
                var field = BitOperations.BitScan(lsb);

                var chain = _chainMasks[field] & board.Pieces[(int)color][(int)Piece.Pawn];
                if (chain != 0)
                {
                    chainedPawns += (int)BitOperations.Count(chain);
                }
            }

            var doubledPawnsScore = doubledPawns * EvaluationConstants.DoubledPawns[(int) GamePhase.Opening] * openingPhase +
                                    doubledPawns * EvaluationConstants.DoubledPawns[(int) GamePhase.Ending] * endingPhase;

            var isolatedPawnsScore = isolatedPawns * EvaluationConstants.IsolatedPawns[(int)GamePhase.Opening] * openingPhase +
                                     isolatedPawns * EvaluationConstants.IsolatedPawns[(int)GamePhase.Ending] * endingPhase;

            var chainedPawnsScore = chainedPawns * EvaluationConstants.ChainedPawns[(int)GamePhase.Opening] * openingPhase +
                                    chainedPawns * EvaluationConstants.ChainedPawns[(int)GamePhase.Ending] * endingPhase;

            var passingPawnsScore = passingPawns * EvaluationConstants.PassingPawns[(int)GamePhase.Opening] * openingPhase +
                                    passingPawns * EvaluationConstants.PassingPawns[(int)GamePhase.Ending] * endingPhase;

            return (int)(doubledPawnsScore + isolatedPawnsScore + chainedPawnsScore + passingPawnsScore);
        }
    }
}
