using System;
using System.Collections.Generic;
using Cosette.Interactive.Commands;

namespace Cosette.Interactive
{
    public class InteractiveConsole
    {
        private Dictionary<string, ICommand> _commands;

        public InteractiveConsole()
        {
            _commands = new Dictionary<string, ICommand>();
            _commands["help"] = new HelpCommand(_commands);
            _commands["quit"] = new QuitCommand();
        }

        public void Run()
        {
            DisplayIntro();
            while (true)
            {
                Console.Write("> ");

                var command = Console.ReadLine()?.ToLower();
                if (command == null)
                {
                    break;
                }

                if (_commands.ContainsKey(command))
                {
                    _commands[command].Run();
                }
                else
                {
                    Console.WriteLine("Command not found");
                }
            }
        }

        private void DisplayIntro()
        {
            Console.WriteLine($"Cosette Chess Engine @ {Environment.OSVersion}");
            Console.WriteLine("Homepage and source code: https://github.com/Tearth/Cosette");
            Console.WriteLine();
            Console.WriteLine("Type \"help\" to display all available commands");
        }
    }
}
