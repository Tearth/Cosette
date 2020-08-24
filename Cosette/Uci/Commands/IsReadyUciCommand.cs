using System;

namespace Cosette.Uci.Commands
{
    public class IsReadyUciCommand : IUciCommand
    {
        private UciClient _uciClient;

        public IsReadyUciCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            Console.WriteLine("readyok");
        }
    }
}