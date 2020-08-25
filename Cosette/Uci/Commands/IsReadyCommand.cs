using System;

namespace Cosette.Uci.Commands
{
    public class IsReadyCommand : IUciCommand
    {
        private UciClient _uciClient;
        private UciGame _uciGame;

        public IsReadyCommand(UciClient uciClient, UciGame uciGame)
        {
            _uciClient = uciClient;
            _uciGame = uciGame;
        }

        public void Run(params string[] parameters)
        {
            Console.WriteLine("readyok");
        }
    }
}