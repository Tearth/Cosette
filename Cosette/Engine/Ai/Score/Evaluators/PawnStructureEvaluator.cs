﻿using Cosette.Engine.Ai.Transposition;
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

        public static int Evaluate(BoardState board, EvaluationStatistics statistics, int openingPhase, int endingPhase)
        {
            var entry = PawnHashTable.Get(board.PawnHash);
            if (entry.IsKeyValid(board.PawnHash))
            {
#if DEBUG
                statistics.PHTHits++;
#endif
                return entry.Score;
            }
#if DEBUG
            else
            {
                statistics.PHTNonHits++;

                if (entry.Key != 0 || entry.Score != 0)
                {
                    statistics.PHTReplacements++;
                }
            }
#endif

            var result = Evaluate(board, Color.White, openingPhase, endingPhase) - 
                         Evaluate(board, Color.Black, openingPhase, endingPhase);

            PawnHashTable.Add(board.PawnHash, (short)result);

#if DEBUG
            statistics.PHTAddedEntries++;
#endif
            return result;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var doubledPawns = 0;
            var isolatedPawns = 0;
            var chainedPawns = 0;
            var passingPawns = 0;

            for (var file = 0; file < 8; file++)
            {
                var friendlyPawnsOnInnerMask = board.Pieces[color][Piece.Pawn] & _innerFileMasks[file];
                var friendlyPawnsOnOuterMask = board.Pieces[color][Piece.Pawn] & _outerFileMasks[file];

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

                var chain = _chainMasks[field] & board.Pieces[color][Piece.Pawn];
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
            var doubledPawnsAdjusted = TaperedEvaluation.AdjustToPhase(doubledPawnsOpeningScore, doubledPawnsEndingScore, openingPhase, endingPhase);

            var isolatedPawnsOpeningScore = isolatedPawns * EvaluationConstants.IsolatedPawns[GamePhase.Opening];
            var isolatedPawnsEndingScore = isolatedPawns * EvaluationConstants.IsolatedPawns[GamePhase.Ending];
            var isolatedPawnsAdjusted = TaperedEvaluation.AdjustToPhase(isolatedPawnsOpeningScore, isolatedPawnsEndingScore, openingPhase, endingPhase);

            var chainedPawnsOpeningScore = chainedPawns * EvaluationConstants.ChainedPawns[GamePhase.Opening];
            var chainedPawnsEndingScore = chainedPawns * EvaluationConstants.ChainedPawns[GamePhase.Ending];
            var chainedPawnsAdjusted = TaperedEvaluation.AdjustToPhase(chainedPawnsOpeningScore, chainedPawnsEndingScore, openingPhase, endingPhase);

            var passingPawnsOpeningScore = passingPawns * EvaluationConstants.PassingPawns[GamePhase.Opening];
            var passingPawnsEndingScore = passingPawns * EvaluationConstants.PassingPawns[GamePhase.Ending];
            var passingPawnsAdjusted = TaperedEvaluation.AdjustToPhase(passingPawnsOpeningScore, passingPawnsEndingScore, openingPhase, endingPhase);

            return doubledPawnsAdjusted + isolatedPawnsAdjusted + chainedPawnsAdjusted + passingPawnsAdjusted;
        }
    }
}
