using System;

namespace Cosette.Uci.Commands
{
    public class DebugCommand : IUciCommand
    {
        private UciClient _uciClient;
        private UciGame _uciGame;

        public DebugCommand(UciClient uciClient, UciGame uciGame)
        {
            _uciClient = uciClient;
            _uciGame = uciGame;
        }

        public void Run(params string[] parameters)
        {
            _uciClient.SetDebugMode(parameters[0] == "on");
        }
    }
}