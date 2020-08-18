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

        public void Run()
        {
            Environment.Exit(0);
        }
    }
}
