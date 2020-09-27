using System;
using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Score;
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

            var standPat = Evaluation.Evaluate(context.BoardState, context.BoardState.ColorToMove);
            if (standPat >= beta)
            {
                context.Statistics.QLeafs++;
                return standPat;
            }

            if (alpha < standPat)
            {
                alpha = standPat;
            }

            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            Span<short> moveValues = stackalloc short[SearchConstants.MaxMovesCount];

            var movesCount = context.BoardState.GetAvailableQMoves(moves);
            MoveOrdering.AssignQValues(context.BoardState, moves, moveValues, movesCount);

            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, moveIndex);

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
