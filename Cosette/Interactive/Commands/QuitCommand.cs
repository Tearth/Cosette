using System;

namespace Cosette.Interactive.Commands
{
    public class QuitCommand : ICommand
    {
        public string Description { get; }

        public QuitCommand()
        {
            Description = "Quit from Cosette";
        }

        public void Run(params string[] parameters)
        {
            Environment.Exit(0);
        }
    }
}
