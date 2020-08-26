using System;
using System.Threading.Tasks;
using Cosette.Engine.Ai;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Uci.Commands
{
    public class GoCommand : IUciCommand
    {
        private UciClient _uciClient;
        private UciGame _uciGame;

        public GoCommand(UciClient uciClient, UciGame uciGame)
        {
            _uciClient = uciClient;
            _uciGame = uciGame;
        }

        public void Run(params string[] parameters)
        {
            Task.Run(SearchEntryPoint);
        }

        private void SearchEntryPoint()
        {
            var statistics = new SearchStatistics();
            var bestMove = _uciGame.SearchBestMove();

            _uciClient.Send($"bestmove {bestMove}");
        }
    }
}