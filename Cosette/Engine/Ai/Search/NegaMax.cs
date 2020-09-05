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
        public static int FindBestMove(BoardState board, int depth, int shallowness, int alpha, int beta, SearchStatistics statistics)
        {
            var originalAlpha = alpha;
            var bestMove = new Move();

            statistics.Nodes++;

            var entry = TranspositionTable.Get(board.Hash);
            if (entry.Type != TranspositionTableEntryType.Invalid && entry.Hash == board.Hash && entry.Depth >= depth)
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
                        alpha = Math.Max(alpha, entry.Score);
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

#if TTCOLLISIONS
            if (entry.Type != TranspositionTableEntryType.Invalid && entry.Key != (byte) (board.Hash >> 56))
            {
                statistics.TTCollisions++;
            }
#endif

            if (board.Pieces[(int) board.ColorToMove][(int)Piece.King] == 0)
            {
                statistics.Leafs++;
                return -EvaluationConstants.Checkmate - depth;
            }

            if (board.IsThreefoldRepetition())
            {
                statistics.Leafs++;
                return EvaluationConstants.ThreefoldRepetition;
            }

            if (depth <= 0)
            {
                statistics.Leafs++;
                return QuiescenceSearch.FindBestMove(board, depth, alpha, beta, statistics);
            }

            Span<Move> moves = stackalloc Move[128];
            Span<int> moveValues = stackalloc int[128];

            var movesCount = board.GetAvailableMoves(moves);
            MoveOrdering.AssignValues(board, moves, moveValues, movesCount, depth, entry);

            var pvs = true;
            for (var i = 0; i < movesCount; i++)
            {
                MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, i);
                board.MakeMove(moves[i]);

                var score = 0;
                if (pvs)
                {
                    score = -FindBestMove(board, depth - 1, shallowness + 1, - beta, -alpha, statistics);
                    pvs = false;
                }
                else
                {
                    var nextDepth = depth - 1;
                    if (shallowness > 3 && i > 3 && moves[i].IsQuiet() && !board.IsKingChecked(board.ColorToMove))
                    {
                        nextDepth -= 1;
                    }

                    score = -FindBestMove(board, nextDepth, shallowness  + 1, - alpha - 1, -alpha, statistics);
                    if (score > alpha && score < beta)
                    {
                        score = -FindBestMove(board, nextDepth, shallowness  + 1, - beta, -score, statistics);
                    }
                }

                if (score > alpha)
                {
                    alpha = score;
                    bestMove = moves[i];
                }

                board.UndoMove(moves[i]);
                if (alpha >= beta)
                {
                    if (moves[i].IsQuiet())
                    {
                        KillerHeuristic.AddKillerMove(moves[i], depth);
                    }

                    if (i == 0)
                    {
                        statistics.BetaCutoffsAtFirstMove++;
                    }

                    statistics.BetaCutoffs++;
                    break;
                }
            }

            if (alpha == -EvaluationConstants.Checkmate - depth + 2 && !board.IsKingChecked(board.ColorToMove))
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
    }
}
