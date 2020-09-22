using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Ai.Time;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Logs;

namespace Cosette.Uci.Commands
{
    public class GoCommand : IUciCommand
    {
        private readonly UciClient _uciClient;

        public GoCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            var whiteTime = GetParameter(parameters, "wtime", int.MaxValue);
            var blackTime = GetParameter(parameters, "btime", int.MaxValue);
            var colorTime = _uciClient.BoardState.ColorToMove == Color.White ? whiteTime : blackTime;
            var maxColorTime = TimeScheduler.CalculateTimeForMove(colorTime, _uciClient.BoardState.MovesCount);

            var depth = GetParameter(parameters, "depth", SearchConstants.MaxDepth);
            var moveTime = GetParameter(parameters, "movetime", 0);
            var nodesCount = GetParameter(parameters, "nodes", ulong.MaxValue);
            var searchMoves = GetParameterWithMoves(parameters, "searchmoves");
            var infiniteFlag = GetFlag(parameters, "infinite");

            var context = new SearchContext(_uciClient.BoardState)
            {
                MaxDepth = depth,
                MaxNodesCount = nodesCount,
                MoveRestrictions = searchMoves
            };

            if (moveTime != 0)
            {
                maxColorTime = int.MaxValue;
                context.WaitForStopCommand = true;

                Task.Run(() =>
                {
                    var stopwatch = Stopwatch.StartNew();
                    while (stopwatch.ElapsedMilliseconds < moveTime)
                    {
                        Task.Delay(1).GetAwaiter().GetResult();
                    }

                    context.AbortSearch = true;
                    context.WaitForStopCommand = false;
                });
            }

            if (infiniteFlag)
            {
                maxColorTime = int.MaxValue;
                context.WaitForStopCommand = true;
            }

            context.MaxTime = maxColorTime;
            Task.Run(() => SearchEntryPoint(context));
        }

        private void SearchEntryPoint(SearchContext context)
        {
            try
            {
                var bestMove = IterativeDeepening.FindBestMove(context);
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
                        var parsedMove = Move.FromTextNotation(_uciClient.BoardState, moveTextNotation);
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