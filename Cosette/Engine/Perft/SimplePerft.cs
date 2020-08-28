using System;
using System.Diagnostics;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Perft.Results;

namespace Cosette.Engine.Perft
{
    public static class SimplePerft
    {
        public static SimplePerftResult Run(BoardState boardState, int depth)
        {
            var stopwatch = Stopwatch.StartNew();
            var leafsCount = Perft(boardState, depth);
            stopwatch.Stop();

            var totalSeconds = stopwatch.Elapsed.TotalSeconds;
            return new SimplePerftResult
            {
                LeafsCount = leafsCount,
                LeafsPerSecond = leafsCount / totalSeconds,
                Time = totalSeconds,
                TimePerLeaf = totalSeconds / leafsCount
            };
        }

        private static ulong Perft(BoardState boardState, int depth)
        {
            if (depth <= 0)
            {
                return 1;
            }

            Span<Move> moves = stackalloc Move[128];
            var movesCount = boardState.GetAvailableMoves(moves);

            ulong nodes = 0;
            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i]);
                if (!boardState.IsKingChecked(ColorOperations.Invert(boardState.ColorToMove)))
                {
                    nodes += Perft(boardState, depth - 1);
                }
                boardState.UndoMove(moves[i]);
            }

            return nodes;
        }
    }
}
