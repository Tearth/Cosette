using System;

namespace Cosette.Uci.Commands
{
    public class IsReadyCommand : IUciCommand
    {
        private UciClient _uciClient;

        public IsReadyCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            Console.WriteLine("readyok");
        }
    }
}