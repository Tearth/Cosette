using System;

namespace Cosette.Uci.Commands
{
    public class QuitCommand : IUciCommand
    {
        private UciClient _uciClient;

        public QuitCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            Environment.Exit(0);
        }
    }
}
