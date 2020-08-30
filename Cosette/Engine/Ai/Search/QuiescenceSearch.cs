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
        public static int FindBestMove(BoardState board, int depth, int alpha, int beta, SearchStatistics statistics)
        {
            if (board.Pieces[(int)board.ColorToMove][(int)Piece.King] == 0)
            {
                statistics.Leafs++;
                return -EvaluationConstants.Pieces[(int)Piece.King] - depth;
            }

            var standPat = Evaluation.Evaluate(board, board.ColorToMove);
            
            if (standPat >= beta)
            {
                return beta;
            }

            if (alpha < standPat)
            {
                alpha = standPat;
            }

            Span<Move> moves = stackalloc Move[128];
            Span<int> moveValues = stackalloc int[128];

            var movesCount = board.GetAvailableQuiescenceMoves(moves);
            MoveOrdering.AssignValues(board, moves, moveValues, movesCount, new TranspositionTableEntry());

            for (var i = 0; i < movesCount; i++)
            {
                MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, i);

                board.MakeMove(moves[i]);
                var score = -FindBestMove(board, depth - 1, -beta, -alpha, statistics);
                board.UndoMove(moves[i]);

                if (score >= beta)
                {
                    statistics.BetaCutoffs++;
                    return beta;
                }

                if (score > alpha)
                {
                    alpha = score;
                    break;
                }
            }

            return alpha;
        }
    }
}
