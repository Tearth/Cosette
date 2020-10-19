using System.Collections.Generic;

namespace Cosette.Interactive.Commands
{
    public class HelpCommand : ICommand
    {
        public string Description { get; }

        private readonly InteractiveConsole _interactiveConsole;
        private readonly Dictionary<string, ICommand> _commands;

        public HelpCommand(InteractiveConsole interactiveConsole, Dictionary<string, ICommand> commands)
        {
            _interactiveConsole = interactiveConsole;
            _commands = commands;

            Description = "Display all available commands";
        }

        public void Run(params string[] parameters)
        {
            _interactiveConsole.WriteLine("List of Cosette commands in the interactive mode:");

            foreach (var command in _commands)
            {
                _interactiveConsole.WriteLine($" - {command.Key} ({command.Value.Description})");
            }
        }
    }
}
