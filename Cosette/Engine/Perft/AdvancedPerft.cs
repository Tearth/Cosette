using System;
using System.Diagnostics;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Perft.Results;

namespace Cosette.Engine.Perft
{
    public static class AdvancedPerft
    {
        public static AdvancedPerftResult Run(BoardState boardState, Color color, int depth)
        {
            var result = new AdvancedPerftResult();
            var stopwatch = Stopwatch.StartNew();
            Perft(boardState, color, depth, result);
            result.Time = stopwatch.Elapsed.TotalSeconds;

            return result;
        }

        private static void Perft(BoardState boardState, Color color, int depth, AdvancedPerftResult result)
        {
            Span<Move> moves = stackalloc Move[128];
            var movesCount = boardState.GetAvailableMoves(moves, color);

            if (depth <= 0)
            {
                result.Leafs++;
                return;
            }

            if (depth == 1)
            {
                UpdateResult(boardState, color, moves, movesCount, result);
                return;
            }

            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i], color);
                if (!boardState.IsKingChecked(color))
                {
                    Perft(boardState, ColorOperations.Invert(color), depth - 1, result);
                }
                boardState.UndoMove(moves[i], color);
            }
        }

        private static void UpdateResult(BoardState boardState, Color color, Span<Move> moves, int movesCount, AdvancedPerftResult result)
        {
            var legalMoveFound = false;
            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i], color);

                if (!boardState.IsKingChecked(color))
                {
                    if ((moves[i].Flags & MoveFlags.Kill) != 0)
                    {
                        result.Captures++;
                    }

                    if ((moves[i].Flags & MoveFlags.Castling) != 0)
                    {
                        result.Castles++;
                    }

                    if ((moves[i].Flags & MoveFlags.EnPassant) != 0)
                    {
                        result.EnPassants++;
                        result.Captures++;
                    }

                    if (boardState.IsKingChecked(ColorOperations.Invert(color)))
                    {
                        result.Checks++;
                    }

                    result.Leafs++;
                    legalMoveFound = true;
                }

                boardState.UndoMove(moves[i], color);
            }

            if (!legalMoveFound)
            {
                result.Checkmates++;
            }
        }
    }
}
