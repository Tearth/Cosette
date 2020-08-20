using System;
using System.Diagnostics;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Interactive.Commands
{
    public class APerftCommand : ICommand
    {
        public string Description { get; }

        public APerftCommand()
        {
            Description = "Test performance of the moves generator with advanced statistics";
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
                var statistics = new APerftStatistics();
                var boardState = new BoardState();
                boardState.SetDefaultState();

                var stopwatch = Stopwatch.StartNew();
                Perft(boardState, Color.White, i, statistics);
                var time = (double)stopwatch.ElapsedMilliseconds / 1000;

                Console.WriteLine($"Depth {i}: {statistics.Nodes} leafs ({time:F} s), Captures: {statistics.Captures}, " +
                                  $"Promotions: {statistics.Promotions}, Castlings: {statistics.Castles}, " +
                                  $"En passants: {statistics.EnPassants}");
            }
        }

        private void Perft(BoardState boardState, Color color, int depth, APerftStatistics statistics)
        {
            Span<Move> moves = stackalloc Move[128];
            var movesCount = boardState.GetAvailableMoves(moves, color);

            if (depth <= 1)
            {
                UpdateStatistics(moves, movesCount, statistics);
                return;
            }

            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i], color);
                Perft(boardState, ColorOperations.Invert(color), depth - 1, statistics);
                boardState.UndoMove(moves[i], color);
            }
        }

        private void UpdateStatistics(Span<Move> moves, int movesCount, APerftStatistics statistics)
        {
            for (var i = 0; i < movesCount; i++)
            {
                if ((moves[i].Flags & MoveFlags.Kill) != 0)
                {
                    statistics.Captures++;
                }

                if ((moves[i].Flags & MoveFlags.Promotion) != 0)
                {
                    statistics.Promotions++;
                }

                if ((moves[i].Flags & MoveFlags.Castling) != 0)
                {
                    statistics.Castles++;
                }

                if ((moves[i].Flags & MoveFlags.EnPassant) != 0)
                {
                    statistics.EnPassants++;
                }
            }

            statistics.Nodes += (ulong) movesCount;
        }
    }
}
