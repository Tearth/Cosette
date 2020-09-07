using System;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Search
{
    public static class QuiescenceSearch
    {
        public static int FindBestMove(BoardState board, int depth, int ply, int alpha, int beta, SearchStatistics statistics)
        {
            statistics.QNodes++;

            if (board.Pieces[(int)board.ColorToMove][(int)Piece.King] == 0)
            {
                statistics.QLeafs++;
                return -EvaluationConstants.Checkmate + ply;
            }

            var standPat = Evaluation.Evaluate(board, board.ColorToMove);
            if (standPat >= beta)
            {
                statistics.QLeafs++;
                return beta;
            }

            if (alpha < standPat)
            {
                alpha = standPat;
            }

            Span<Move> moves = stackalloc Move[128];
            Span<int> moveValues = stackalloc int[128];

            var movesCount = board.GetAvailableQMoves(moves);
            MoveOrdering.AssignQValues(board, moves, moveValues, movesCount);

            for (var i = 0; i < movesCount; i++)
            {
                MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, i);

                board.MakeMove(moves[i]);
                var score = -FindBestMove(board, depth - 1, ply + 1, -beta, -alpha, statistics);
                board.UndoMove(moves[i]);

                if (score >= beta)
                {
                    if (i == 0)
                    {
                        statistics.QBetaCutoffsAtFirstMove++;
                    }

                    statistics.QBetaCutoffs++;
                    return beta;
                }

                if (score > alpha)
                {
                    alpha = score;
                }
            }

            return alpha;
        }
    }
}
