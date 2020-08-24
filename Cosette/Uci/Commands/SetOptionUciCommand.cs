using System;

namespace Cosette.Uci.Commands
{
    public class SetOptionUciCommand : IUciCommand
    {
        private UciClient _uciClient;

        public SetOptionUciCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            // Ignore options for now, engine doesn't support any
        }
    }
}