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
            _uciGame.SearchContext.AbortSearch = true;
            _uciGame.SearchContext.WaitForStopCommand = false;
        }
    }
}