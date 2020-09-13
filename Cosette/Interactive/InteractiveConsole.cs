using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Cosette.Interactive.Commands;

namespace Cosette.Interactive
{
    public class InteractiveConsole
    {
        private Dictionary<string, ICommand> _commands;
        private List<string> _keywords;
        private List<string> _symbols;

        private ConsoleColor _keywordColor;
        private ConsoleColor _numberColor;
        private ConsoleColor _symbolColor;

        private string _regex = @"(\s|\,|\:|\(|\))";

        public InteractiveConsole()
        {
            _commands = new Dictionary<string, ICommand>();
            _commands["help"] = new HelpCommand(this, _commands);
            _commands["magic"] = new MagicCommand(this);
            _commands["aperft"] = new AdvancedPerftCommand(this);
            _commands["dperft"] = new DividedPerftCommand(this);
            _commands["perft"] = new SimplePerftCommand(this);
            _commands["benchmark"] = new BenchmarkCommand(this);
            _commands["verify"] = new VerifyCommand(this);
            _commands["uci"] = new UciCommand(this);
            _commands["quit"] = new QuitCommand(this);

            _keywords = new List<string> 
            { 
                // UCI keywords
                "id", "name", "author", "uciok", "readyok", "bestmove", "copyprotection", 
                "registration", "info", "depth", "seldepth", "time", "nodes", "pv", "multipv", "score", "cp", "mate", 
                "lowerbound", "upperbound", "currmove", "currmovenumber", "hashfull", "nps", "tbhits", "cpuload", 
                "string", "refutation", "currline",

                // Custom keywords
                "Depth", "Score", "Best", "Time"
            };
            _symbols = new List<string> { "%", "s", "MN/s" };

            _keywordColor = ConsoleColor.Cyan;
            _numberColor = ConsoleColor.Yellow;
            _symbolColor = ConsoleColor.Yellow;

            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);
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

        public void WriteLine()
        {
            Console.WriteLine();
        }

        public void WriteLine(string message)
        {
            var splitMessage = Regex.Split(message, _regex);
            foreach (var chunk in splitMessage.Where(p => !string.IsNullOrEmpty(p)))
            {
                if (_keywords.Contains(chunk))
                {
                    Console.ForegroundColor = _keywordColor;
                }
                else if (_symbols.Contains(chunk))
                {
                    Console.ForegroundColor = _symbolColor;
                }
                else if (float.TryParse(chunk, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                {
                    Console.ForegroundColor = _numberColor;
                }

                Console.Write(chunk);
                Console.ResetColor();
            }

            Console.WriteLine();
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
