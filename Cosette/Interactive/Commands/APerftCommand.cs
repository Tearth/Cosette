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
                                  $"Checkmates: {statistics.Checkmates}, Castlings: {statistics.Castles}, " +
                                  $"En passants: {statistics.EnPassants}, Checks: {statistics.Checks}");
            }
        }

        private void Perft(BoardState boardState, Color color, int depth, APerftStatistics statistics)
        {
            Span<Move> moves = stackalloc Move[128];
            var movesCount = boardState.GetAvailableMoves(moves, color);

            if (depth <= 1)
            {
                UpdateStatistics(boardState, color, moves, movesCount, statistics);
                return;
            }

            var legalMoveFound = false;
            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i], color);
                if (!boardState.IsKingChecked(color))
                {
                    Perft(boardState, ColorOperations.Invert(color), depth - 1, statistics);
                    legalMoveFound = true;
                }
                boardState.UndoMove(moves[i], color);
            }

            if (!legalMoveFound)
            {
                //statistics.Checkmates++;
            }
        }

        private void UpdateStatistics(BoardState boardState, Color color, Span<Move> moves, int movesCount, APerftStatistics statistics)
        {
            var legalMoveFound = false;
            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i], color);

                if (!boardState.IsKingChecked(color))
                {
                    if ((moves[i].Flags & MoveFlags.Kill) != 0)
                    {
                        statistics.Captures++;
                    }

                    if ((moves[i].Flags & MoveFlags.Castling) != 0)
                    {
                        statistics.Castles++;
                    }

                    if ((moves[i].Flags & MoveFlags.EnPassant) != 0)
                    {
                        statistics.EnPassants++;
                        statistics.Captures++;
                    }

                    if (boardState.IsKingChecked(ColorOperations.Invert(color)))
                    {
                        statistics.Checks++;
                    }

                    statistics.Nodes++;
                    legalMoveFound = true;
                }
                
                boardState.UndoMove(moves[i], color);
            }

            if (!legalMoveFound)
            {
                statistics.Checkmates++;
            }
        }
    }
}
