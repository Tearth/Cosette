using System;

namespace Cosette.Uci.Commands
{
    public class QuitCommand : IUciCommand
    {
        private UciClient _uciClient;
        private UciGame _uciGame;

        public QuitCommand(UciClient uciClient, UciGame uciGame)
        {
            _uciClient = uciClient;
            _uciGame = uciGame;
        }

        public void Run(params string[] parameters)
        {
            Environment.Exit(0);
        }
    }
}
