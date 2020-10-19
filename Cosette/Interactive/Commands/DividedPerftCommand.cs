using Cosette.Engine.Board;
using Cosette.Engine.Perft;

namespace Cosette.Interactive.Commands
{
    public class DividedPerftCommand : ICommand
    {
        public string Description { get; }
        private readonly InteractiveConsole _interactiveConsole;

        public DividedPerftCommand(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;
            Description = "Test performance of the divided moves generator";
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

            var result = DividedPerft.Run(boardState, depth);
            foreach (var move in result.LeafsCount)
            {
                _interactiveConsole.WriteLine($"{move.Key}: {move.Value}");
            }

            _interactiveConsole.WriteLine();
            _interactiveConsole.WriteLine($"Total leafs: {result.TotalLeafsCount}");
        }
    }
}