using System;
using System.Diagnostics;
using System.Linq;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Moves.Magic;
using Cosette.Engine.Perft;

namespace Cosette.Interactive.Commands
{
    public class SimplePerftCommand : ICommand
    {
        public string Description { get; }
        private InteractiveConsole _interactiveConsole;

        public SimplePerftCommand(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;
            Description = "Test performance of the moves generator";
        }

        public void Run(params string[] parameters)
        {
            if (parameters.Length < 1 || !int.TryParse(parameters[0], out var depth))
            {
                _interactiveConsole.WriteLine("No depth specified");
                return;
            }

            var boardState = new BoardState();
            boardState.SetDefaultState();

            for (var i = 0; i <= depth; i++)
            {
                var result = SimplePerft.Run(boardState, i);
                var megaLeafsPerSecond = result.LeafsPerSecond / 1_000_000;
                var nanosecondsPerLeaf = result.TimePerLeaf * 1_000_000_000;

                _interactiveConsole.WriteLine($"Depth {i} - Leafs: {result.LeafsCount} ({megaLeafsPerSecond:F} ML/s), " +
                                              $"Time: {result.Time:F} s, Time per leaf: {nanosecondsPerLeaf:F} ns");
            }
        }
    }
}