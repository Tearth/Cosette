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

            if (ply > statistics.SelectiveDepth)
            {
                statistics.SelectiveDepth = ply;
            }

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

            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            Span<int> moveValues = stackalloc int[SearchConstants.MaxMovesCount];

            var movesCount = board.GetAvailableQMoves(moves);
            MoveOrdering.AssignQValues(board, moves, moveValues, movesCount);

            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, moveIndex);

                board.MakeMove(moves[moveIndex]);
                var score = -FindBestMove(board, depth - 1, ply + 1, -beta, -alpha, statistics);
                board.UndoMove(moves[moveIndex]);

                if (score >= beta)
                {
#if DEBUG
                    if (moveIndex == 0)
                    {
                        statistics.QBetaCutoffsAtFirstMove++;
                    }
                    else
                    {
                        statistics.QBetaCutoffsNotAtFirstMove++;
                    }
#endif

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
