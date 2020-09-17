using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cosette.Engine.Ai;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Logs;

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
            var depth = GetParameter(parameters, "depth", 0);
            var moveTime = GetParameter(parameters, "movetime", 0);
            var infiniteFlag = GetFlag(parameters, "infinite");

            if (moveTime != 0)
            {
                whiteTime = int.MaxValue;
                blackTime = int.MaxValue;
                IterativeDeepening.WaitForStopCommand = true;

                Task.Run(() =>
                {
                    var stopwatch = Stopwatch.StartNew();
                    while (stopwatch.ElapsedMilliseconds < moveTime)
                    {
                        Task.Delay(20).GetAwaiter().GetResult();
                    }

                    IterativeDeepening.AbortSearch = true;
                    IterativeDeepening.WaitForStopCommand = false;
                });
            }

            if (infiniteFlag)
            {
                whiteTime = int.MaxValue;
                blackTime = int.MaxValue;
                IterativeDeepening.WaitForStopCommand = true;
            }

            Task.Run(() => SearchEntryPoint(whiteTime, blackTime, depth));
        }

        private void SearchEntryPoint(int whiteTime, int blackTime, int depth)
        {
            try
            {
                var bestMove = _uciGame.SearchBestMove(whiteTime, blackTime, depth);
                _uciClient.Send($"bestmove {bestMove}");
            }
            catch (Exception ex)
            {
                LogManager.LogError(ex.ToString());
                throw;
            }
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