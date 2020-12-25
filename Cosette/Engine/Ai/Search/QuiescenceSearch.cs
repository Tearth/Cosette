using System;
using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Search
{
    public static class QuiescenceSearch
    {
        public static int FindBestMove(SearchContext context, int depth, int ply, int alpha, int beta)
        {
            context.Statistics.QNodes++;

            if (ply > context.Statistics.SelectiveDepth)
            {
                context.Statistics.SelectiveDepth = ply;
            }

            if (context.BoardState.Pieces[context.BoardState.ColorToMove][Piece.King] == 0)
            {
                context.Statistics.QLeafs++;
                return -EvaluationConstants.Checkmate + ply;
            }

            var standPat = 0;

            var evaluationEntry = EvaluationHashTable.Get(context.BoardState.Hash);
            if (evaluationEntry.IsKeyValid(context.BoardState.Hash))
            {
                standPat = evaluationEntry.Score;

#if DEBUG
                context.Statistics.EvaluationStatistics.EHTHits++;
#endif
            }
            else
            {
                standPat = Evaluation.Evaluate(context.BoardState, true, context.Statistics.EvaluationStatistics);
                EvaluationHashTable.Add(context.BoardState.Hash, (short)standPat);

#if DEBUG
                context.Statistics.EvaluationStatistics.EHTNonHits++;
                context.Statistics.EvaluationStatistics.EHTAddedEntries++;

                if (evaluationEntry.Key != 0 || evaluationEntry.Score != 0)
                {
                    context.Statistics.EvaluationStatistics.EHTReplacements++;
                }
#endif
            }    

            if (standPat >= beta)
            {
                context.Statistics.QLeafs++;
                return standPat;
            }

            if (standPat > alpha)
            {
                alpha = standPat;
            }

            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            Span<short> moveValues = stackalloc short[SearchConstants.MaxMovesCount];

            var movesCount = context.BoardState.GetAvailableCaptureMoves(moves);
            MoveOrdering.AssignQValues(context.BoardState, moves, moveValues, movesCount);

            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, moveIndex);

                if (moveValues[moveIndex] < -50)
                {
                    break;
                }

                context.BoardState.MakeMove(moves[moveIndex]);
                var score = -FindBestMove(context, depth - 1, ply + 1, -beta, -alpha);
                context.BoardState.UndoMove(moves[moveIndex]);

                if (score > alpha)
                {
                    alpha = score;

                    if (alpha >= beta)
                    {
#if DEBUG
                        if (moveIndex == 0)
                        {
                            context.Statistics.QBetaCutoffsAtFirstMove++;
                        }
                        else
                        {
                            context.Statistics.QBetaCutoffsNotAtFirstMove++;
                        }
#endif

                        context.Statistics.QBetaCutoffs++;
                        break;
                    }
                }
            }

            return alpha;
        }
    }
}
