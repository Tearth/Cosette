using System;

namespace Cosette.Interactive.Commands
{
    public class QuitCommand : ICommand
    {
        public string Description { get; }
        private InteractiveConsole _interactiveConsole;

        public QuitCommand(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;
            Description = "Quit from Cosette";
        }

        public void Run(params string[] parameters)
        {
            Environment.Exit(0);
        }
    }
}
