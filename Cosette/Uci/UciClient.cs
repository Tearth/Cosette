using System;
using System.Collections.Generic;
using System.Linq;
using Cosette.Uci.Commands;

namespace Cosette.Uci
{
    public class UciClient
    {
        private Dictionary<string, IUciCommand> _commands;

        public UciClient()
        {
            _commands = new Dictionary<string, IUciCommand>();
            _commands["quit"] = new QuitUciCommand(this);
            _commands["setoption"] = new SetOptionUciCommand(this);
        }

        public void Run()
        {
            SendName();
            SendAuthor();
            SendOptions();
            RunCommandsLoop();
        }

        private void SendName()
        {
            Console.WriteLine("id name Cosette");
        }

        private void SendAuthor()
        {
            Console.WriteLine("id author Tearth");
        }

        private void SendOptions()
        {
            Console.WriteLine("uciok");
        }

        private void RunCommandsLoop()
        {
            while (true)
            {
                var input = Console.ReadLine();
                var splitInput = input.Split(' ');
                var command = splitInput[0].ToLower();
                var parameters = splitInput.Skip(1).ToArray();

                if (_commands.ContainsKey(command))
                {
                    _commands[command].Run(parameters);
                }
            }
        }
    }
}
