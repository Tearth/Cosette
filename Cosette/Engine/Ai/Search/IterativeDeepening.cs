using System;
using System.Diagnostics;
using Cosette.Engine.Ai.Ordering;
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
            var expectedExecutionTime = 0;

            var alpha = SearchConstants.MinValue;
            var beta = SearchConstants.MaxValue;

            TranspositionTable.Clear();
            HistoryHeuristic.Clear();

            var lastTotalNodesCount = 1ul;
            var timeLimit = TimeScheduler.CalculateTimeForMove(remainingTime, moveNumber);
            var stopwatch = Stopwatch.StartNew();
            for (var i = 1; expectedExecutionTime <= timeLimit && Math.Abs(statistics.Score) != EvaluationConstants.Checkmate; i++)
            {
                statistics.Clear();

                statistics.Board = board;
                statistics.Depth = i;
                statistics.Score = NegaMax.FindBestMove(board, i, 0, alpha, beta, false, statistics);
                statistics.SearchTime = (ulong) stopwatch.ElapsedMilliseconds;
                statistics.PrincipalVariationMovesCount = GetPrincipalVariation(board, statistics.PrincipalVariation, 0);

                OnSearchUpdate?.Invoke(null, statistics);

                var ratio = (float) statistics.TotalNodes / lastTotalNodesCount;
                expectedExecutionTime = (int)(statistics.SearchTime * ratio);
                lastTotalNodesCount = statistics.TotalNodes;
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

            var enemyColor = ColorOperations.Invert(board.ColorToMove);
            var king = board.Pieces[(int)enemyColor][(int) Piece.King];
            var kingField = BitOperations.BitScan(king);

            if (board.IsFieldAttacked(enemyColor, (byte) kingField))
            {
                board.UndoMove(entry.BestMove);
                return movesCount;
            }

            movesCount = GetPrincipalVariation(board, moves, movesCount + 1);
            board.UndoMove(entry.BestMove);

            return movesCount;
        }
    }
}
