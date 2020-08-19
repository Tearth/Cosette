using System;
using System.Diagnostics;
using System.Linq;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Moves.Magic;

namespace Cosette.Interactive.Commands
{
    public class PerftCommand : ICommand
    {
        public string Description { get; }

        public PerftCommand()
        {
            Description = "Test performance of the moves generator";
        }

        public void Run(params string[] parameters)
        {
            if (parameters.Length < 1 || !int.TryParse(parameters[0], out var depth))
            {
                Console.WriteLine("No depth specified");
                return;
            }

            for (var i = 1; i <= depth; i++)
            {
                var boardState = new BoardState();
                boardState.SetDefaultState();

                var stopwatch = Stopwatch.StartNew();
                var leafsCount = Perft(boardState, Color.White, i);
                var time = (double) stopwatch.ElapsedMilliseconds / 1000;

                Console.WriteLine($"Depth {i}: {leafsCount} leafs ({time:F} s)");
            }
        }

        private ulong Perft(BoardState boardState, Color color, int depth)
        {
            Span<Move> moves = stackalloc Move[128];
            var movesCount = boardState.GetAvailableMoves(moves, color);

            if (depth <= 1)
            {
                return (ulong) movesCount;
            }

            ulong nodes = 0;
            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i]);
                nodes += Perft(boardState, ColorOperations.Invert(color), depth - 1);
                boardState.UndoMove(moves[i]);
            }

            return nodes;
        }
    }
}