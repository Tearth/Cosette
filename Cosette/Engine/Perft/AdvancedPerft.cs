using System;
using System.Diagnostics;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Perft.Results;

namespace Cosette.Engine.Perft
{
    public static class AdvancedPerft
    {
        public static AdvancedPerftResult Run(BoardState boardState, int depth)
        {
            var result = new AdvancedPerftResult();
            var stopwatch = Stopwatch.StartNew();
            Perft(boardState, depth, result);
            result.Time = stopwatch.Elapsed.TotalSeconds;

            return result;
        }

        private static void Perft(BoardState boardState, int depth, AdvancedPerftResult result)
        {
            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            var movesCount = boardState.GetAvailableMoves(moves);

            if (depth <= 0)
            {
                result.Leafs++;
                return;
            }

            if (depth == 1)
            {
                UpdateResult(boardState, moves, movesCount, result);
                return;
            }

            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i]);
                if (!boardState.IsKingChecked(ColorOperations.Invert(boardState.ColorToMove)))
                {
                    Perft(boardState, depth - 1, result);
                }
                boardState.UndoMove(moves[i]);
            }
        }

        private static void UpdateResult(BoardState boardState, Span<Move> moves, int movesCount, AdvancedPerftResult result)
        {
            var legalMoveFound = false;
            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i]);

                if (!boardState.IsKingChecked(ColorOperations.Invert(boardState.ColorToMove)))
                {
                    if (((byte)moves[i].Flags & MoveFlagFields.Capture) != 0)
                    {
                        result.Captures++;
                    }

                    if (moves[i].Flags == MoveFlags.KingCastle || moves[i].Flags == MoveFlags.QueenCastle)
                    {
                        result.Castles++;
                    }

                    if ((moves[i].Flags & MoveFlags.EnPassant) != 0)
                    {
                        result.EnPassants++;
                        result.Captures++;
                    }

                    if (boardState.IsKingChecked(boardState.ColorToMove))
                    {
                        result.Checks++;
                    }

                    result.Leafs++;
                    legalMoveFound = true;
                }

                boardState.UndoMove(moves[i]);
            }

            if (!legalMoveFound)
            {
                result.Checkmates++;
            }
        }
    }
}
