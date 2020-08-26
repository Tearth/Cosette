using System;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai
{
    public static class NegaMax
    {
        public static int FindBestMove(BoardState board, Color color, int depth, out Move bestMove, SearchStatistics statistics)
        {
            var max = int.MinValue;
            bestMove = new Move();

            if (board.Pieces[(int) color][(int) Piece.King] == 0)
            {
                // Add depth here
                return -BoardConstants.PieceValues[(int) Color.White][(int) Piece.King];
            }

            if (depth <= 0)
            {
                statistics.Leafs++;
                return Evaluation.Evaluate(board, color);
            }

            Span<Move> moves = stackalloc Move[128];
            var movesCount = board.GetAvailableMoves(moves, color);

            for (var i = 0; i < movesCount; i++)
            {
                board.MakeMove(moves[i], color);

                var score = -FindBestMove(board, ColorOperations.Invert(color), depth - 1, out _, statistics);
                if (score > max)
                {
                    max = score;
                    bestMove = moves[i];
                }

                board.UndoMove(moves[i], color);
            }

            statistics.Nodes++;
            return max;
        }
    }
}
