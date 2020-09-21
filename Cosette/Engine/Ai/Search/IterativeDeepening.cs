using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
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
        public static bool AbortSearch { get; set; }
        public static bool WaitForStopCommand { get; set; }
        public static List<Move> MoveRestrictions { get; set; }
        public static ulong MaxNodesCount { get; set; }
        public static event EventHandler<SearchStatistics> OnSearchUpdate;

        public static Move FindBestMove(BoardState board, int remainingTime, int depth, int moveNumber)
        {
            var statistics = new SearchStatistics();
            var expectedExecutionTime = 0;

            var alpha = SearchConstants.MinValue;
            var beta = SearchConstants.MaxValue;

            TranspositionTable.Clear();
            HistoryHeuristic.Clear();

            var timeLimit = TimeScheduler.CalculateTimeForMove(remainingTime, moveNumber);
            var stopwatch = Stopwatch.StartNew();
            var lastTotalNodesCount = 100ul;
            var bestMove = new Move();

            AbortSearch = false;

            for (var currentDepth = 1; currentDepth < SearchConstants.MaxDepth && !IsScoreCheckmate(statistics.Score); currentDepth++)
            {
                if (expectedExecutionTime > timeLimit)
                {
                    break;
                }

                statistics.Clear();

                statistics.Board = board;
                statistics.Depth = currentDepth;
                statistics.Score = NegaMax.FindBestMove(board, currentDepth, 0, alpha, beta, true, true, statistics);
                statistics.SearchTime = (ulong)stopwatch.ElapsedMilliseconds;
                statistics.PrincipalVariationMovesCount = GetPrincipalVariation(board, statistics.PrincipalVariation, 0);

                bestMove = statistics.PrincipalVariation[0];
                stopwatch.Stop();

                if (AbortSearch)
                {
                    break;
                }

                OnSearchUpdate?.Invoke(null, statistics);
                stopwatch.Start();

                if (currentDepth == depth)
                {
                    break;
                }

                var ratio = (float)statistics.TotalNodes / lastTotalNodesCount;
                expectedExecutionTime = (int)(statistics.SearchTime * ratio);
                lastTotalNodesCount = statistics.TotalNodes;
            }

            while (WaitForStopCommand)
            {
                Task.Delay(50).GetAwaiter().GetResult();
            }

            if (AbortSearch)
            {
                TranspositionTable.Clear();
                AbortSearch = false;
            }

            return bestMove;
        }

        public static bool IsScoreCheckmate(int score)
        {
            var scoreAbs = Math.Abs(score);
            return scoreAbs > EvaluationConstants.Checkmate - SearchConstants.MaxDepth && 
                   scoreAbs < EvaluationConstants.Checkmate + SearchConstants.MaxDepth;
        }

        public static int GetMovesToCheckmate(int score)
        {
            return Math.Abs(Math.Abs(score) - EvaluationConstants.Checkmate) / 2;
        }

        private static int GetPrincipalVariation(BoardState board, Move[] moves, int movesCount)
        {
            var entry = TranspositionTable.Get(board.Hash);
            if (entry.Type != TranspositionTableEntryType.ExactScore || entry.Hash != board.Hash || movesCount >= SearchConstants.MaxDepth)
            {
                return movesCount;
            }

            moves[movesCount] = entry.BestMove;

            board.MakeMove(entry.BestMove);

            var enemyColor = ColorOperations.Invert(board.ColorToMove);
            var king = board.Pieces[(int)enemyColor][(int)Piece.King];
            var kingField = BitOperations.BitScan(king);

            if (board.IsFieldAttacked(enemyColor, (byte)kingField))
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
