using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Cosette.Interactive.Commands;

namespace Cosette.Interactive
{
    public class InteractiveConsole
    {
        private readonly Dictionary<string, ICommand> _commands;
        private readonly List<string> _symbols;

        private readonly ConsoleColor _keywordColor;
        private readonly ConsoleColor _moveColor;
        private readonly ConsoleColor _numberColor;
        private readonly ConsoleColor _symbolColor;

        private readonly Regex _splitRegex = new Regex(@"(\s|\,|\:|\(|\))", RegexOptions.Compiled);
        private readonly Regex _moveRegex = new Regex(@"([a-h][1-8]){2}[nbrq]?", RegexOptions.Compiled);

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
            _commands["evaluate"] = new EvaluateCommand(this);
            _commands["uci"] = new UciCommand(this);
            _commands["quit"] = new QuitCommand(this);

            _symbols = new List<string> { "%", "s", "ns", "MN/s", "ML/s" };

            _keywordColor = ConsoleColor.Cyan;
            _moveColor = ConsoleColor.Red;
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
            if (Console.IsOutputRedirected)
            {
                Console.WriteLine(message);
            }
            else
            {
                var splitMessage = _splitRegex.Split(message);
                foreach (var chunk in splitMessage.Where(p => !string.IsNullOrEmpty(p)))
                {
                    if (_moveRegex.IsMatch(chunk))
                    {
                        Console.ForegroundColor = _moveColor;
                    }
                    else if (_symbols.Contains(chunk))
                    {
                        Console.ForegroundColor = _symbolColor;
                    }
                    else if (float.TryParse(chunk, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                    {
                        Console.ForegroundColor = _numberColor;
                    }
                    else
                    {
                        Console.ForegroundColor = _keywordColor;
                    }

                    Console.Write(chunk);
                    Console.ResetColor();
                }

                Console.WriteLine();
            }
        }

        private void DisplayIntro()
        {
            var runtimeVersion = $"{Environment.Version.Major}.{Environment.Version.Minor}.{Environment.Version.Build}";
            var executableHash = GetExecutableHash();

            Console.WriteLine($"Cosette v2.0 (Darkness), 19.10.2020 @ {Environment.OSVersion} (.NET Core {runtimeVersion})");
            Console.WriteLine("Distributed under AGPL license, homepage and source code: https://github.com/Tearth/Cosette");
            Console.WriteLine($"Executable hash: {executableHash}");
            Console.WriteLine();
            Console.WriteLine("\"The blunders are all there on the board, waiting to be made.\" ~ Savielly Tartakower");
            Console.WriteLine();
            Console.WriteLine("Type \"help\" to display all available commands");
        }

        private string GetExecutableHash()
        {
            var md5 = new MD5CryptoServiceProvider();
            using (var streamReader = new StreamReader(Process.GetCurrentProcess().MainModule.FileName))
            {
                md5.ComputeHash(streamReader.BaseStream);
            }

            var hashBuilder = new StringBuilder();
            foreach (var b in md5.Hash)
            {
                hashBuilder.Append(b.ToString("x2"));
            }

            return hashBuilder.ToString();
        }
    }
}
