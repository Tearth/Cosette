using System;
using System.Diagnostics;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Time;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Search
{
    public static class IterativeDeepening
    {
        public static event EventHandler<SearchStatistics> OnSearchUpdate;

        public static Move FindBestMove(BoardState board, int remainingTime, int moveNumber)
        {
            var statistics = new SearchStatistics();
            var lastNodesCount = 1ul;
            var expectedExecutionTime = 0;

            var alpha = SearchConstants.MinValue;
            var beta = SearchConstants.MaxValue;
            TranspositionTable.Clear();

            var timeLimit = TimeScheduler.CalculateTimeForMove(remainingTime, moveNumber);
            var stopwatch = Stopwatch.StartNew();
            for (var i = 1; expectedExecutionTime <= timeLimit && Math.Abs(statistics.Score) != EvaluationConstants.Checkmate; i++)
            {
                statistics.Clear();

                statistics.Board = board;
                statistics.Depth = i;
                statistics.Score = NegaMax.FindBestMove(board, i, alpha, beta, statistics);

                statistics.SearchTime = (ulong) stopwatch.ElapsedMilliseconds;
                statistics.NodesPerSecond = (ulong)(statistics.Nodes / ((float)statistics.SearchTime / 1000));
                statistics.BranchingFactor = (float)statistics.Nodes / lastNodesCount;
                statistics.PrincipalVariationMovesCount = GetPrincipalVariation(board, statistics.PrincipalVariation, 0);

                OnSearchUpdate?.Invoke(null, statistics);
                lastNodesCount = statistics.Nodes;

                expectedExecutionTime = (int)(statistics.SearchTime * statistics.BranchingFactor);
            }

            return statistics.PrincipalVariation[0];
        }

        public static int GetPrincipalVariation(BoardState board, Move[] moves, int movesCount)
        {
            var entry = TranspositionTable.Get(board.Hash);
            if (entry.Type != TranspositionTableEntryType.ExactScore || entry.Hash != board.Hash || movesCount >= 32)
            {
                return movesCount;
            }

            moves[movesCount] = entry.BestMove;

            board.MakeMove(entry.BestMove);
            if (board.Pieces[(int) ColorOperations.Invert(board.ColorToMove)][(int)Piece.King] == 0)
            {
                board.UndoMove(entry.BestMove);
                return movesCount - 1;
            }

            movesCount = GetPrincipalVariation(board, moves, movesCount + 1);
            board.UndoMove(entry.BestMove);

            return movesCount;
        }
    }
}
