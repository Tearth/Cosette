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
            var leafsCount = Perft(boardState, Color.White, depth);
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

        private static ulong Perft(BoardState boardState, Color color, int depth)
        {
            if (depth <= 1)
            {
                return 1;
            }

            Span<Move> moves = stackalloc Move[128];
            var movesCount = boardState.GetAvailableMoves(moves, color);

            ulong nodes = 0;
            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i], color);
                if (!boardState.IsKingChecked(color))
                {
                    nodes += Perft(boardState, ColorOperations.Invert(color), depth - 1);
                }
                boardState.UndoMove(moves[i], color);
            }

            return nodes;
        }
    }
}
