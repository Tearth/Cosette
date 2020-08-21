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

        public SimplePerftCommand()
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

            var boardState = new BoardState();
            boardState.SetDefaultState();

            for (var i = 1; i <= depth; i++)
            {
                var result = SimplePerft.Run(boardState, Color.White, i);
                var megaLeafsPerSecond = result.LeafsPerSecond / 1_000_000;
                var nanosecondsPerLeaf = result.TimePerLeaf * 1_000_000_000;

                Console.WriteLine($"Depth {i} - Leafs: {result.LeafsCount}, Time: {result.Time:F} s, " +
                                  $"LPS: {megaLeafsPerSecond:F} ML/s, TPL: {nanosecondsPerLeaf:F} ns");
            }
        }
    }
}