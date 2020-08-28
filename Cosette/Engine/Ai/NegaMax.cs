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
            bestMove = new Move();
            statistics.Nodes++;

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

            for (var i = 0; i < movesCount; i++)
            {
                board.MakeMove(moves[i]);

                var score = -FindBestMove(board, depth - 1, -beta, -alpha, out _, statistics);
                if (score >= beta)
                {
                    board.UndoMove(moves[i]);
                    statistics.BetaCutoffs++;
                    return beta;
                }

                if (score > alpha)
                {
                    alpha = score;
                    bestMove = moves[i];
                }

                board.UndoMove(moves[i]);
            }

            return alpha;
        }
    }
}
