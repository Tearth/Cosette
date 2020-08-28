using System;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai
{
    public static class NegaMax
    {
        public static int FindBestMove(BoardState board, int depth, int alpha, int beta, out Move bestMove, SearchStatistics statistics)
        {
            var originalAlpha = alpha;

            bestMove = new Move();
            statistics.Nodes++;

            if (TranspositionTable.Exists(board.Hash))
            {
                var entry = TranspositionTable.Get(board.Hash);
                if (entry.Depth >= depth)
                {
                    switch (entry.Type)
                    {
                        case TranspositionTableEntryType.ExactScore:
                        {
                            bestMove = entry.BestMove;
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

                    statistics.TTHits++;
                }

                if (alpha >= beta)
                {
                    bestMove = entry.BestMove;
                    return entry.Score;
                }
            }

            if (board.Pieces[(int) board.ColorToMove][(int)Piece.King] == 0)
            {
                statistics.Leafs++;
                return -BoardConstants.PieceValues[(int)Color.White][(int)Piece.King] - depth;
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

                var score = -FindBestMove(board, depth - 1, -beta, -alpha, out _, statistics);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = moves[i];
                }

                alpha = Math.Max(alpha, score);

                if (alpha >= beta)
                {
                    board.UndoMove(moves[i]);
                    statistics.BetaCutoffs++;
                    break;
                }

                board.UndoMove(moves[i]);
            }

            var entryType = bestScore <= originalAlpha ? TranspositionTableEntryType.UpperBound :
                            bestScore >= beta ? TranspositionTableEntryType.LowerBound :
                            TranspositionTableEntryType.ExactScore;
            TranspositionTable.Add(board.Hash, depth, bestScore, bestMove, entryType);

            return bestScore;
        }
    }
}
