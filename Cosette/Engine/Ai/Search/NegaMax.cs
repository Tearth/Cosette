using System;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Search
{
    public static class NegaMax
    {
        public static int FindBestMove(BoardState board, int depth, int alpha, int beta, SearchStatistics statistics)
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
                return -EvaluationConstants.Pieces[(int)Piece.King] - depth;
            }

            if (depth <= 0)
            {
                statistics.Leafs++;
                return Evaluation.Evaluate(board, board.ColorToMove);
            }

            Span<Move> moves = stackalloc Move[128];
            var movesCount = board.GetAvailableMoves(moves);

            var bestScore = int.MinValue;
            for (var i = 0; i < movesCount; i++)
            {
                board.MakeMove(moves[i]);

                var score = -FindBestMove(board, depth - 1, -beta, -alpha, statistics);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = moves[i];
                }

                alpha = Math.Max(alpha, score);

                board.UndoMove(moves[i]);
                if (alpha >= beta)
                {
                    statistics.BetaCutoffs++;
                    break;
                }
            }

            var entryType = bestScore <= originalAlpha ? TranspositionTableEntryType.UpperBound :
                            bestScore >= beta ? TranspositionTableEntryType.LowerBound :
                            TranspositionTableEntryType.ExactScore;
            TranspositionTable.Add(board.Hash, (byte)depth, (short)bestScore, bestMove, entryType);

            return bestScore;
        }
    }
}
