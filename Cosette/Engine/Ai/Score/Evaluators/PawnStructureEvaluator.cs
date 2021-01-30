using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class PawnStructureEvaluator
    {
        public static int Evaluate(BoardState board, EvaluationStatistics statistics, int openingPhase, int endingPhase)
        {
            var entry = PawnHashTable.Get(board.PawnHash);
            if (entry.IsKeyValid(board.PawnHash))
            {
#if DEBUG
                statistics.PHTHits++;
#endif
                return TaperedEvaluation.AdjustToPhase(entry.OpeningScore, entry.EndingScore, openingPhase, endingPhase);
            }
#if DEBUG
            else
            {
                statistics.PHTNonHits++;

                if (entry.Key != 0 || entry.OpeningScore != 0 || entry.EndingScore != 0)
                {
                    statistics.PHTReplacements++;
                }
            }
#endif

            var (openingWhiteScore, endingWhiteScore) = Evaluate(board, Color.White, openingPhase, endingPhase);
            var (openingBlackScore, endingBlackScore) = Evaluate(board, Color.Black, openingPhase, endingPhase);

            var openingScore = openingWhiteScore - openingBlackScore;
            var endingScore = endingWhiteScore - endingBlackScore;
            var result = TaperedEvaluation.AdjustToPhase(openingScore, endingScore, openingPhase, endingPhase);

            PawnHashTable.Add(board.PawnHash, (short)openingScore, (short)endingScore);

#if DEBUG
            statistics.PHTAddedEntries++;
#endif
            return result;
        }

        public static int EvaluateWithoutCache(BoardState board, EvaluationStatistics statistics, int openingPhase, int endingPhase)
        {
            var (openingWhiteScore, endingWhiteScore) = Evaluate(board, Color.White, openingPhase, endingPhase);
            var (openingBlackScore, endingBlackScore) = Evaluate(board, Color.Black, openingPhase, endingPhase);

            var openingScore = openingWhiteScore - openingBlackScore;
            var endingScore = endingWhiteScore - endingBlackScore;

            return TaperedEvaluation.AdjustToPhase(openingScore, endingScore, openingPhase, endingPhase);
        }

        private static (int openingScore, int endingScore) Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var doubledPawns = 0;
            var isolatedPawns = 0;
            var chainedPawns = 0;
            var passingPawns = 0;

            for (var file = 0; file < 8; file++)
            {
                var friendlyPawnsOnInnerMask = board.Pieces[color][Piece.Pawn] & FilePatternGenerator.GetPatternForFile(file);
                var friendlyPawnsOnOuterMask = board.Pieces[color][Piece.Pawn] & OuterFilesPatternGenerator.GetPatternForFile(file);

                var pawnsCount = BitOperations.Count(friendlyPawnsOnInnerMask);
                if (pawnsCount > 1)
                {
                    doubledPawns += (int)(pawnsCount - 1);
                }

                if (friendlyPawnsOnInnerMask != 0)
                {
                    if (friendlyPawnsOnOuterMask == 0)
                    {
                        isolatedPawns += (int)BitOperations.Count(pawnsCount);
                    }
                }
            }

            var pieces = board.Pieces[color][Piece.Pawn];
            while (pieces != 0)
            {
                var lsb = BitOperations.GetLsb(pieces);
                var field = BitOperations.BitScan(lsb);
                pieces = BitOperations.PopLsb(pieces);

                var chain = ChainPatternGenerator.GetPattern(field) & board.Pieces[color][Piece.Pawn];
                if (chain != 0)
                {
                    chainedPawns += (int)BitOperations.Count(chain);
                }

                if (board.IsFieldPassing(color, field))
                {
                    passingPawns++;
                }
            }

            var doubledPawnsOpeningScore = doubledPawns * EvaluationConstants.DoubledPawns[GamePhase.Opening];
            var doubledPawnsEndingScore = doubledPawns * EvaluationConstants.DoubledPawns[GamePhase.Ending];

            var isolatedPawnsOpeningScore = isolatedPawns * EvaluationConstants.IsolatedPawns[GamePhase.Opening];
            var isolatedPawnsEndingScore = isolatedPawns * EvaluationConstants.IsolatedPawns[GamePhase.Ending];

            var chainedPawnsOpeningScore = chainedPawns * EvaluationConstants.ChainedPawns[GamePhase.Opening];
            var chainedPawnsEndingScore = chainedPawns * EvaluationConstants.ChainedPawns[GamePhase.Ending];

            var passingPawnsOpeningScore = passingPawns * EvaluationConstants.PassingPawns[GamePhase.Opening];
            var passingPawnsEndingScore = passingPawns * EvaluationConstants.PassingPawns[GamePhase.Ending];

            var openingScore = doubledPawnsOpeningScore + isolatedPawnsOpeningScore + chainedPawnsOpeningScore + passingPawnsOpeningScore;
            var endingScore = doubledPawnsEndingScore + isolatedPawnsEndingScore + chainedPawnsEndingScore + passingPawnsEndingScore;

            return (openingScore, endingScore);
        }
    }
}
