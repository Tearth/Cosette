using System;
using System.Collections.Generic;
using System.Linq;
using Cosette.Uci.Commands;

namespace Cosette.Uci
{
    public class UciClient
    {
        private UciGame _uciGame;
        private Dictionary<string, IUciCommand> _commands;

        public UciClient()
        {
            _uciGame = new UciGame();

            _commands = new Dictionary<string, IUciCommand>();
            _commands["quit"] = new QuitCommand(this, _uciGame);
            _commands["setoption"] = new SetOptionCommand(this, _uciGame);
            _commands["isready"] = new IsReadyCommand(this, _uciGame);
            _commands["ucinewgame"] = new UciNewGameCommand(this, _uciGame);
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
