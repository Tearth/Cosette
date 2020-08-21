using System;
using System.Diagnostics;
using System.Linq;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Moves.Magic;

namespace Cosette.Interactive.Commands
{
    public class DPerftCommand : ICommand
    {
        public string Description { get; }

        public DPerftCommand()
        {
            Description = "Test performance of the moves generator split into categories";
        }

        public void Run(params string[] parameters)
        {
            if (parameters.Length < 1 || !int.TryParse(parameters[0], out var depth))
            {
                Console.WriteLine("No depth specified");
                return;
            }

            var boardState = new BoardState();
            boardState.SetDefaultState();

            Span<Move> moves = stackalloc Move[128];
            var movesCount = boardState.GetAvailableMoves(moves, Color.White);

            var totalLeafsCount = 0ul;
            for (var i = 0; i < movesCount; i++)
            {
                var from = Position.FromFieldIndex(moves[i].From).ToString();
                var to = Position.FromFieldIndex(moves[i].To).ToString();

                boardState.MakeMove(moves[i], Color.White);
                var leafsCount = Perft(boardState, Color.Black, depth);
                boardState.UndoMove(moves[i], Color.White);

                Console.WriteLine($"{from}{to}: {leafsCount}");
                totalLeafsCount += leafsCount;
            }

            Console.WriteLine();
            Console.WriteLine($"Total leafs: {totalLeafsCount}");
        }

        private ulong Perft(BoardState boardState, Color color, int depth)
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