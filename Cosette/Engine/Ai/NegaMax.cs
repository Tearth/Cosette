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
            bestMove = new Move();

            if (depth <= 0)
            {
                statistics.LeafsCount++;
                return Evaluation.Evaluate(board, color);
            }

            var max = int.MinValue;
            Span<Move> moves = stackalloc Move[128];
            var movesCount = board.GetAvailableMoves(moves, color);

            for (var i = 0; i < movesCount; i++)
            {
                board.MakeMove(moves[i], color);
                if (!board.IsKingChecked(color))
                {
                    var score = -FindBestMove(board, ColorOperations.Invert(color), depth - 1, out Move potentialBestMove, statistics);
                    if (score > max)
                    {
                        max = score;
                        bestMove = moves[i];
                    }
                }
                board.UndoMove(moves[i], color);
            }

            return max;
        }
    }
}
