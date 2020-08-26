﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai
{
    public static class IterativeDeepening
    {
        public static event EventHandler<SearchStatistics> OnSearchUpdate;

        public static Move FindBestMove(BoardState board, Color color)
        {
            var bestMove = new Move();
            var statistics = new SearchStatistics();
            var lastNodesCount = 1ul;

            var alpha = SearchConstants.MinValue;
            var beta = SearchConstants.MaxValue;

            var stopwatch = Stopwatch.StartNew();
            for (var i = 1; i < 5; i++)
            {
                statistics.Clear();

                statistics.Depth = i;
                statistics.Score = NegaMax.FindBestMove(board, color, i, alpha, beta, out bestMove, statistics);

                statistics.SearchTime = (ulong) stopwatch.ElapsedMilliseconds;
                statistics.NodesPerSecond = (ulong)(statistics.Nodes / ((float)statistics.SearchTime / 1000));
                statistics.BranchingFactor = (int)(statistics.Nodes / lastNodesCount);

                OnSearchUpdate?.Invoke(null, statistics);
                lastNodesCount = statistics.Nodes;
            }

            return bestMove;
        }
    }
}
