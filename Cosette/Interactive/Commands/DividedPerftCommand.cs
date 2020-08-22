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
    public class DividedPerftCommand : ICommand
    {
        public string Description { get; }

        public DividedPerftCommand()
        {
            Description = "Test performance of the divided moves generator";
        }

        public void Run(params string[] parameters)
        {
            if (parameters.Length < 1 || !int.TryParse(parameters[0], out var depth))
            {
                Console.WriteLine("No depth specified");
                return;
            }

            GC.TryStartNoGCRegion(1024 * 1024 * 16);

            var boardState = new BoardState();
            boardState.SetDefaultState();

            var result = DividedPerft.Run(boardState, Color.White, depth);
            foreach (var move in result.LeafsCount)
            {
                Console.WriteLine($"{move.Key}: {move.Value}");
            }

            Console.WriteLine();
            Console.WriteLine($"Total leafs: {result.TotalLeafsCount}");
            GC.EndNoGCRegion();
        }

        
    }
}