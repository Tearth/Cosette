using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Search
{
    public static class IterativeDeepening
    {
        public static event EventHandler<SearchStatistics> OnSearchUpdate;

        public static Move FindBestMove(SearchContext context)
        {
            TranspositionTable.SetEntriesAsOld();
            HistoryHeuristic.Clear();

            var expectedExecutionTime = 0;
            var alpha = SearchConstants.MinValue;
            var beta = SearchConstants.MaxValue;
            var lastSearchTime = 10ul;
            var bestMove = Move.Empty;
            var stopwatch = Stopwatch.StartNew();

            for (var depth = 1; expectedExecutionTime <= context.MaxTime && depth <= context.MaxDepth && !IsScoreCheckmate(context.Statistics.Score); depth++)
            {
                context.Statistics.Clear();

                context.Statistics.Board = context.BoardState;
                context.Statistics.Depth = depth;
                context.Statistics.Score = NegaMax.FindBestMove(context, depth, 0, alpha, beta, true, true);
                context.Statistics.SearchTime = (ulong)stopwatch.ElapsedMilliseconds;
                context.Statistics.PrincipalVariationMovesCount = GetPrincipalVariation(context.BoardState, context.Statistics.PrincipalVariation, 0);
                bestMove = context.Statistics.PrincipalVariation[0];

                if (context.AbortSearch)
                {
                    break;
                }

                OnSearchUpdate?.Invoke(null, context.Statistics);

                var ratio = (float)context.Statistics.SearchTime / lastSearchTime;
                expectedExecutionTime = (int)(context.Statistics.SearchTime * ratio);
                lastSearchTime = context.Statistics.SearchTime;
            }

            while (context.WaitForStopCommand)
            {
                Task.Delay(1).GetAwaiter().GetResult();
            }

            context.AbortSearch = false;
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
            if ((entry.Flags & TranspositionTableEntryFlags.ExactScore) != 0 && entry.IsKeyValid(board.Hash) && movesCount < SearchConstants.MaxDepth)
            {
                moves[movesCount] = entry.BestMove;
                board.MakeMove(entry.BestMove);

                var enemyColor = ColorOperations.Invert(board.ColorToMove);
                var king = board.Pieces[enemyColor][Piece.King];
                var kingField = BitOperations.BitScan(king);

                if (board.IsFieldAttacked(enemyColor, (byte)kingField))
                {
                    board.UndoMove(entry.BestMove);
                    return movesCount;
                }

                movesCount = GetPrincipalVariation(board, moves, movesCount + 1);
                board.UndoMove(entry.BestMove);
            }

            return movesCount;
        }
    }
}
