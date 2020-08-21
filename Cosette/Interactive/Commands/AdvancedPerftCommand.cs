using System;
using System.Diagnostics;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Perft;

namespace Cosette.Interactive.Commands
{
    public class AdvancedPerftCommand : ICommand
    {
        public string Description { get; }

        public AdvancedPerftCommand()
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

            var boardState = new BoardState();
            boardState.SetDefaultState();

            for (var i = 1; i <= depth; i++)
            {
                var result = AdvancedPerft.Run(boardState, i);

                Console.WriteLine($"Depth {i}: {result.Nodes} leafs ({result.Time:F} s), Captures: {result.Captures}, " +
                                  $"Checkmates: {result.Checkmates}, Castlings: {result.Castles}, " +
                                  $"En passants: {result.EnPassants}, Checks: {result.Checks}");
            }
        }
    }
}
