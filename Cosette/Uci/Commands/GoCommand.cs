using System;
using System.Collections.Generic;
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
            var whiteTime = GetParameter(parameters, "wtime", int.MaxValue);
            var blackTime = GetParameter(parameters, "btime", int.MaxValue);
            var depth = GetParameter(parameters, "depth", SearchConstants.MaxDepth);
            var moveTime = GetParameter(parameters, "movetime", 0);
            var nodesCount = GetParameter(parameters, "nodes", ulong.MaxValue);
            var searchMoves = GetParameterWithMoves(parameters, "searchmoves");
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

            IterativeDeepening.MoveRestrictions = searchMoves;
            IterativeDeepening.MaxNodesCount = nodesCount;
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
        
        private List<Move> GetParameterWithMoves(string[] parameters, string name)
        {
            var movesList = new List<Move>();
            for (var i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == name)
                {
                    for (var moveIndex = i + 1; moveIndex < parameters.Length; moveIndex++)
                    {
                        var moveTextNotation = parameters[moveIndex];
                        var parsedMove = Move.FromTextNotation(_uciGame.BoardState, moveTextNotation);
                        movesList.Add(parsedMove);
                    }
                }
            }

            if (movesList.Count == 0)
            {
                return null;
            }

            return movesList;
        }
        
        private bool GetFlag(string[] parameters, string name)
        {
            return parameters.Contains(name);
        }
    }
}