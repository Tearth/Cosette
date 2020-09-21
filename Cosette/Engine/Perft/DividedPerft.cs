using System;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Perft.Results;

namespace Cosette.Engine.Perft
{
    public static class DividedPerft
    {
        public static DividedPerftResult Run(BoardState boardState, int depth)
        {
            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            var movesCount = boardState.GetAvailableMoves(moves);

            var result = new DividedPerftResult();
            if (depth <= 0)
            {
                return result;
            }

            for (var i = 0; i < movesCount; i++)
            {
                var from = Position.FromFieldIndex(moves[i].From).ToString();
                var to = Position.FromFieldIndex(moves[i].To).ToString();

                boardState.MakeMove(moves[i]);
                var leafsCount = Perft(boardState, depth - 1);
                boardState.UndoMove(moves[i]);

                result.LeafsCount[$"{from}{to}"] = leafsCount;
                result.TotalLeafsCount += leafsCount;
            }

            return result;
        }

        private static ulong Perft(BoardState boardState, int depth)
        {
            if (depth <= 0)
            {
                return 1;
            }

            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
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
