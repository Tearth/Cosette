using System;

namespace Cosette.Uci.Commands
{
    public class SetOptionCommand : IUciCommand
    {
        private UciClient _uciClient;

        public SetOptionCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            // Ignore options for now, engine doesn't support any
        }
    }
}