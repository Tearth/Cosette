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
        public static int FindBestMove(BoardState board, int depth, int alpha, int beta, int nullMoves, SearchStatistics statistics)
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

            if (depth >= 2 && nullMoves < 2 && !board.IsKingChecked(board.ColorToMove))
            {
                board.MakeNullMove();
                var score = -FindBestMove(board, depth - 3, -beta, -beta + 1, nullMoves + 1, statistics);
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
            for (var i = 0; i < movesCount; i++)
            {
                MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, i);
                board.MakeMove(moves[i]);

                var score = 0;
                if (pvs)
                {
                    score = -FindBestMove(board, depth - 1, -beta, -alpha, 0, statistics);
                    pvs = false;
                }
                else
                {
                    if (depth >= 3 && i >= 3 && moves[i].IsQuiet() && !board.IsKingChecked(board.ColorToMove))
                    {
                        score = -FindBestMove(board, depth - 2, -alpha - 1, -alpha, 0, statistics);
                    }
                    else
                    {
                        score = alpha + 1;
                    }

                    if (score > alpha)
                    {
                        score = -FindBestMove(board, depth - 1, -alpha - 1, -alpha, 0, statistics);
                        if (score > alpha && score < beta)
                        {
                            score = -FindBestMove(board, depth - 1, -beta, -score, 0, statistics);
                        }
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
                        KillerHeuristic.AddKillerMove(moves[i], board.ColorToMove, depth);
                        HistoryHeuristic.AddHistoryMove(board.ColorToMove, moves[i].From, moves[i].To, depth * depth);
                    }

                    if (i < 1)
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
