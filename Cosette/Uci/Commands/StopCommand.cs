using Cosette.Engine.Ai.Search;

namespace Cosette.Uci.Commands
{
    public class StopCommand : IUciCommand
    {
        private readonly UciClient _uciClient;

        public StopCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            _uciClient.SearchContext.AbortSearch = true;
            _uciClient.SearchContext.WaitForStopCommand = false;
        }
    }
}