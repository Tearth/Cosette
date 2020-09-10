using System;
using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Search
{
    public static class NegaMax
    {
        public static int FindBestMove(BoardState board, int depth, int ply, int alpha, int beta, bool allowNullMove, bool pvNode, SearchStatistics statistics)
        {
            var originalAlpha = alpha;
            var bestMove = new Move();

            statistics.Nodes++;

            var entry = TranspositionTable.Get(board.Hash);
            if (entry.Hash == board.Hash && entry.Depth >= depth)
            {
                statistics.TTHits++;
                switch (entry.Type)
                {
                    case TranspositionTableEntryType.ExactScore:
                    {
                        return entry.Score;
                    }

                    case TranspositionTableEntryType.LowerBound:
                    {
                        if (entry.Score > alpha)
                        {
                            alpha = entry.Score;
                            bestMove = entry.BestMove;
                        }

                        break;
                    }

                    case TranspositionTableEntryType.UpperBound:
                    {
                        beta = Math.Min(beta, entry.Score);
                        break;
                    }
                }

                if (alpha >= beta)
                {
                    statistics.Leafs++;
                    return entry.Score;
                }
            }

#if DEBUG
            if (entry.Type != TranspositionTableEntryType.Invalid && entry.Hash !=board.Hash)
            {
                statistics.TTCollisions++;
            }
#endif

            if (board.Pieces[(int)board.ColorToMove][(int)Piece.King] == 0)
            {
                statistics.Leafs++;
                return -EvaluationConstants.Checkmate + ply;
            }

            if (board.IsThreefoldRepetition())
            {
                statistics.Leafs++;
                return EvaluationConstants.ThreefoldRepetition;
            }

            if (depth <= 0)
            {
                statistics.Leafs++;
                return QuiescenceSearch.FindBestMove(board, depth, ply, alpha, beta, statistics);
            }

            if (NullWindowCanBeApplied(board, depth, allowNullMove, pvNode))
            {
                board.MakeNullMove();
                var score = -FindBestMove(board, depth - 1 - SearchConstants.NullWindowDepthReduction, ply + 1, -beta, -beta + 1, false, pvNode, statistics);
                board.UndoNullMove();

                if (score >= beta)
                {
                    statistics.BetaCutoffs++;
                    return score;
                }
            }

            Span<Move> moves = stackalloc Move[128];
            Span<int> moveValues = stackalloc int[128];

            var movesCount = board.GetAvailableMoves(moves);
            MoveOrdering.AssignValues(board, moves, moveValues, movesCount, depth, entry);

            var pvs = true;
            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, moveIndex);
                board.MakeMove(moves[moveIndex]);

                var score = 0;
                if (pvs)
                {
                    score = -FindBestMove(board, depth - 1, ply + 1, -beta, -alpha, allowNullMove, true, statistics);
                    pvs = false;
                }
                else
                {
                    var reducedDepth = depth;
                    if (LMRCanBeApplied(board, depth, moveIndex, moves))
                    {
                        reducedDepth -= SearchConstants.LMRDepthReduction;
                    }

                    score = -FindBestMove(board, reducedDepth - 1, ply + 1, -alpha - 1, -alpha, allowNullMove, false, statistics);
                    if (score > alpha)
                    {
                        score = -FindBestMove(board, reducedDepth - 1, ply + 1, -beta, -alpha, allowNullMove, false, statistics);
                    }
                }

                if (score > alpha)
                {
                    alpha = score;
                    bestMove = moves[moveIndex];
                }

                board.UndoMove(moves[moveIndex]);
                if (alpha >= beta)
                {
                    if (moves[moveIndex].IsQuiet())
                    {
                        KillerHeuristic.AddKillerMove(moves[moveIndex], board.ColorToMove, depth);
                        HistoryHeuristic.AddHistoryMove(board.ColorToMove, moves[moveIndex].From, moves[moveIndex].To, depth * depth);
                    }

#if DEBUG
                    if (moveIndex == 0)
                    {
                        statistics.BetaCutoffsAtFirstMove++;
                    }
#endif

                    statistics.BetaCutoffs++;
                    break;
                }
            }

            if (alpha == -EvaluationConstants.Checkmate + ply + 2 && !board.IsKingChecked(board.ColorToMove))
            {
                statistics.Leafs++;
                return 0;
            }

            var entryType = alpha <= originalAlpha ? TranspositionTableEntryType.UpperBound :
                            alpha >= beta ? TranspositionTableEntryType.LowerBound :
                            TranspositionTableEntryType.ExactScore;
            TranspositionTable.Add(board.Hash, (byte)depth, (short)alpha, bestMove, entryType);

            return alpha;
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool NullWindowCanBeApplied(BoardState board, int depth, bool allowNullMove, bool pvNode)
        {
            return !pvNode && allowNullMove && depth > SearchConstants.NullWindowMinimalDepth && 
                   board.GetGamePhase() == GamePhase.Opening && !board.IsKingChecked(board.ColorToMove);
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        private static bool LMRCanBeApplied(BoardState board, int depth, int moveIndex, Span<Move> moves)
        {
            return depth > SearchConstants.LMRMinimalDepth && moveIndex > SearchConstants.LMRMovesWithoutReduction &&
                   moves[moveIndex].IsQuiet() && !board.IsKingChecked(board.ColorToMove);
        }
    }
}
