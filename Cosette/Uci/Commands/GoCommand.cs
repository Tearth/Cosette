using System;
using System.Linq;
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
            var whiteTime = GetParameter(parameters, "wtime", 1);
            var blackTime = GetParameter(parameters, "btime", 1);

            Task.Run(() => SearchEntryPoint(whiteTime, blackTime));
        }

        private void SearchEntryPoint(int whiteTime, int blackTime)
        {
            var bestMove = _uciGame.SearchBestMove(whiteTime, blackTime);
            _uciClient.Send($"bestmove {bestMove}");
        }

        private T GetParameter<T>(string[] parameters, string name, T defaultValue)
        {
            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == name)
                {
                    return (T) Convert.ChangeType(parameters[i + 1], typeof(T));
                }
            }

            return defaultValue;
        }

        private bool GetFlag(string[] parameters, string name)
        {
            return parameters.Contains(name);
        }
    }
}