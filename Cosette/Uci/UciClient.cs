using System;
using System.Collections.Generic;
using System.Linq;
using Cosette.Engine.Ai;
using Cosette.Uci.Commands;

namespace Cosette.Uci
{
    public class UciClient
    {
        private UciGame _uciGame;
        private bool _debugMode;

        private Dictionary<string, IUciCommand> _commands;

        public UciClient()
        {
            _uciGame = new UciGame();
            _debugMode = true; // Arena workaround

            _commands = new Dictionary<string, IUciCommand>();
            _commands["quit"] = new QuitCommand(this, _uciGame);
            _commands["setoption"] = new SetOptionCommand(this, _uciGame);
            _commands["isready"] = new IsReadyCommand(this, _uciGame);
            _commands["ucinewgame"] = new UciNewGameCommand(this, _uciGame);
            _commands["position"] = new PositionCommand(this, _uciGame);
            _commands["debug"] = new DebugCommand(this, _uciGame);
            _commands["go"] = new GoCommand(this, _uciGame);

            IterativeDeepening.OnSearchUpdate += OnSearchUpdate;
        }

        public void Run()
        {
            SendName();
            SendAuthor();
            SendOptions();
            RunCommandsLoop();
        }

        public void Send(string command)
        {
            Console.WriteLine(command);
        }

        public (string Command, string[] parameters) Receive()
        {
            var input = Console.ReadLine();
            var splitInput = input.Split(' ');
            var command = splitInput[0].ToLower();
            var parameters = splitInput.Skip(1).ToArray();

            return (command, parameters);
        }

        public void SetDebugMode(bool state)
        {
            _debugMode = state;
        }

        private void SendName()
        {
            Send("id name Cosette");
        }

        private void SendAuthor()
        {
            Send("id author Tearth");
        }

        private void SendOptions()
        {
            Send("uciok");
        }

        private void RunCommandsLoop()
        {
            while (true)
            {
                var (command, parameters) = Receive();
                if (_commands.ContainsKey(command))
                {
                    _commands[command].Run(parameters);
                }
            }
        }

        private void OnSearchUpdate(object sender, SearchStatistics stats)
        {
            Send($"info depth {stats.Depth} time {stats.SearchTime} score cp {stats.Score} nodes {stats.Nodes} nps {stats.NodesPerSecond}");

            if (_debugMode)
            {
                Send($"info string depth {stats.Depth} bfactor {stats.BranchingFactor}");
            }
        }
    }
}
