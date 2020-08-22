using System;
using System.Collections.Generic;
using System.Linq;
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
            _commands["magic"] = new MagicCommand();
            _commands["aperft"] = new AdvancedPerftCommand();
            _commands["dperft"] = new DividedPerftCommand();
            _commands["perft"] = new SimplePerftCommand();
            _commands["benchmark"] = new BenchmarkCommand();
            _commands["quit"] = new QuitCommand();
        }

        public void Run()
        {
            DisplayIntro();
            while (true)
            {
                Console.Write("> ");

                var input = Console.ReadLine();
                var splitInput = input.Split(' ');
                var command = splitInput[0].ToLower();
                var parameters = splitInput.Skip(1).ToArray();

                if (_commands.ContainsKey(command))
                {
                    _commands[command].Run(parameters);
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
