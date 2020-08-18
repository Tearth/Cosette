using System;
using System.Collections.Generic;

namespace Cosette.Interactive.Commands
{
    public class HelpCommand : ICommand
    {
        private Dictionary<string, ICommand> _commands;

        public HelpCommand(Dictionary<string, ICommand> commands)
        {
            _commands = commands;
        }

        public void Run()
        {
            Console.WriteLine("List of Cosette commands in the interactive mode");

            foreach (var command in _commands)
            {
                Console.WriteLine($" - {command.Key}");
            }
        }
    }
}
