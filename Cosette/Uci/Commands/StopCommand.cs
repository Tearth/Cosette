using System;
using Cosette.Engine.Ai.Search;

namespace Cosette.Uci.Commands
{
    public class StopCommand : IUciCommand
    {
        private UciClient _uciClient;
        private UciGame _uciGame;

        public StopCommand(UciClient uciClient, UciGame uciGame)
        {
            _uciClient = uciClient;
            _uciGame = uciGame;
        }

        public void Run(params string[] parameters)
        {
            IterativeDeepening.AbortSearch = true;
            IterativeDeepening.WaitForStopCommand = false;
        }
    }
}