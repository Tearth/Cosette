using System;
using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Search
{
    public static class NegaMax
    {
        public static int FindBestMove(SearchContext context, int depth, int ply, int alpha, int beta, bool allowNullMove, bool pvNode)
        {
            if (context.Statistics.Nodes >= context.MaxNodesCount)
            {
                context.AbortSearch = true;
                return 0;
            }

            if (context.AbortSearch)
            {
                return 0;
            }

            context.Statistics.Nodes++;

            if (context.BoardState.Pieces[context.BoardState.ColorToMove][Piece.King] == 0)
            {
                context.Statistics.Leafs++;
                return -EvaluationConstants.Checkmate + ply;
            }

            if (context.BoardState.IsThreefoldRepetition())
            {
                context.Statistics.Leafs++;
                return EvaluationConstants.ThreefoldRepetition;
            }

            if (context.BoardState.IsFiftyMoveRuleDraw())
            {
                context.Statistics.Leafs++;
                
                if (context.BoardState.IsKingChecked(ColorOperations.Invert(context.BoardState.ColorToMove)))
                {
                    return EvaluationConstants.Checkmate + ply;
                }
                
                return EvaluationConstants.ThreefoldRepetition;
            }

            if (depth <= 0)
            {
                context.Statistics.Leafs++;
                return QuiescenceSearch.FindBestMove(context, depth, ply, alpha, beta);
            }

            var originalAlpha = alpha;
            var bestMove = Move.Empty;

            var entry = TranspositionTable.Get(context.BoardState.Hash);
            if (entry.IsKeyValid(context.BoardState.Hash))
            {
#if DEBUG
                context.Statistics.TTHits++;
#endif

                if (entry.Depth >= depth)
                {
                    entry.Score = (short)TranspositionTable.TTToRegularScore(entry.Score, ply);
                    switch (entry.Flags)
                    {
                        case TranspositionTableEntryFlags.AlphaScore:
                        {
                            if (entry.Score < beta)
                            {
                                beta = entry.Score;
                            }

                            break;
                        }

                        case TranspositionTableEntryFlags.ExactScore:
                        {
                            if (entry.Age == context.TranspositionTableEntryAge)
                            {
                                return entry.Score;
                            }

                            break;
                        }

                        case TranspositionTableEntryFlags.BetaScore:
                        {
                            if (entry.Score > alpha)
                            {
                                alpha = entry.Score;
                                bestMove = entry.BestMove;
                            }

                            break;
                        }
                    }

                    if (alpha >= beta)
                    {
                        context.Statistics.BetaCutoffs++;
                        return entry.Score;
                    }
                }
            }
#if DEBUG
            else
            {
                context.Statistics.TTNonHits++;

                if (entry.Flags != TranspositionTableEntryFlags.Invalid)
                {
                    context.Statistics.TTCollisions++;
                }
            }
#endif
            
            if (NullWindowCanBeApplied(context.BoardState, depth, allowNullMove, pvNode))
            {
                context.BoardState.MakeNullMove();
                var score = -FindBestMove(context, depth - 1 - SearchConstants.NullWindowDepthReduction, ply + 1, -beta, -beta + 1, false, pvNode);
                context.BoardState.UndoNullMove();

                if (score >= beta)
                {
                    context.Statistics.BetaCutoffs++;
                    return score;
                }
            }
            
            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            Span<short> moveValues = stackalloc short[SearchConstants.MaxMovesCount];

            var movesCount = context.BoardState.GetAvailableMoves(moves);
            MoveOrdering.AssignValues(context.BoardState, moves, moveValues, movesCount, depth, entry);

            var pvs = true;
            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, moveIndex);

                if (context.MoveRestrictions != null && ply == 0)
                {
                    if (!context.MoveRestrictions.Contains(moves[moveIndex]))
                    {
                        continue;
                    }
                }

                context.BoardState.MakeMove(moves[moveIndex]);

                var score = 0;
                if (pvs)
                {
                    score = -FindBestMove(context, depth - 1, ply + 1, -beta, -alpha, allowNullMove, true);
                    pvs = false;
                }
                else
                {
                    var reducedDepth = depth;
                    if (LMRCanBeApplied(context.BoardState, depth, moveIndex, moves))
                    {
                        reducedDepth = LMRGetReducedDepth(depth, pvNode);
                    }

                    score = -FindBestMove(context, reducedDepth - 1, ply + 1, -alpha - 1, -alpha, allowNullMove, false);
                    if (score > alpha)
                    {
                        score = -FindBestMove(context, depth - 1, ply + 1, -beta, -alpha, allowNullMove, false);
                    }
                }

                if (score > alpha)
                {
                    alpha = score;
                    bestMove = moves[moveIndex];
                }

                context.BoardState.UndoMove(moves[moveIndex]);
                if (alpha >= beta)
                {
                    if (moves[moveIndex].IsQuiet())
                    {
                        KillerHeuristic.AddKillerMove(moves[moveIndex], context.BoardState.ColorToMove, depth);
                        HistoryHeuristic.AddHistoryMove(context.BoardState.ColorToMove, moves[moveIndex].From, moves[moveIndex].To, depth);
                    }

#if DEBUG
                    if (moveIndex == 0)
                    {
                        context.Statistics.BetaCutoffsAtFirstMove++;
                    }
                    else
                    {
                        context.Statistics.BetaCutoffsNotAtFirstMove++;
                    }
#endif

                    context.Statistics.BetaCutoffs++;
                    break;
                }
            }

            if (alpha == -EvaluationConstants.Checkmate + ply + 2 && !context.BoardState.IsKingChecked(context.BoardState.ColorToMove))
            {
                TranspositionTable.Add(context.BoardState.Hash, (byte)depth, 0,
                    (byte)context.TranspositionTableEntryAge, bestMove, TranspositionTableEntryFlags.ExactScore);

                return 0;
            }

            if (entry.Age < context.TranspositionTableEntryAge || entry.Depth < depth)
            {
                var valueToSave = TranspositionTable.RegularToTTScore(alpha, ply);
                var entryType = alpha <= originalAlpha ? TranspositionTableEntryFlags.AlphaScore :
                    alpha >= beta ? TranspositionTableEntryFlags.BetaScore :
                    TranspositionTableEntryFlags.ExactScore;

                TranspositionTable.Add(context.BoardState.Hash, (byte)depth, (short)valueToSave, 
                    (byte)context.TranspositionTableEntryAge, bestMove, entryType);

#if DEBUG
                context.Statistics.TTEntries++;
#endif
            }

            return alpha;
        }

        private static bool NullWindowCanBeApplied(BoardState board, int depth, bool allowNullMove, bool pvNode)
        {
            return !pvNode && allowNullMove && depth >= SearchConstants.NullWindowMinimalDepth && 
                   board.GetGamePhase() == GamePhase.Opening && !board.IsKingChecked(board.ColorToMove);
        }

        private static bool LMRCanBeApplied(BoardState board, int depth, int moveIndex, Span<Move> moves)
        {
            return depth >= SearchConstants.LMRMinimalDepth && moveIndex > SearchConstants.LMRMovesWithoutReduction &&
                   moves[moveIndex].IsQuiet() && !board.IsKingChecked(board.ColorToMove);
        }

        private static int LMRGetReducedDepth(int depth, bool pvNode)
        {
            return pvNode ? 
                depth - SearchConstants.LMRPvNodeDepthReduction : 
                depth - depth / SearchConstants.LMRNonPvNodeDepthDivisor;
        }
    }
}
