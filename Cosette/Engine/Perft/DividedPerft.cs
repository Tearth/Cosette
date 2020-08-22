using System;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Perft.Results;

namespace Cosette.Engine.Perft
{
    public static class DividedPerft
    {
        public static DividedPerftResult Run(BoardState boardState, Color color, int depth)
        {
            Span<Move> moves = stackalloc Move[128];
            var movesCount = boardState.GetAvailableMoves(moves, color);

            var result = new DividedPerftResult();
            for (var i = 0; i < movesCount; i++)
            {
                var from = Position.FromFieldIndex(moves[i].From).ToString();
                var to = Position.FromFieldIndex(moves[i].To).ToString();

                boardState.MakeMove(moves[i], color);
                var leafsCount = Perft(boardState, ColorOperations.Invert(color), depth);
                boardState.UndoMove(moves[i], color);

                result.LeafsCount[$"{from}{to}"] = leafsCount;
                result.TotalLeafsCount += leafsCount;
            }

            return result;
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
