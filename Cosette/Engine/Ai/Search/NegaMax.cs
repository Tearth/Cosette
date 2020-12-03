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
        public static int FindBestMove(SearchContext context, int depth, int ply, int alpha, int beta)
        {
            var friendlyKingInCheck = context.BoardState.IsKingChecked(context.BoardState.ColorToMove);
            return FindBestMove(context, depth, ply, alpha, beta, true, friendlyKingInCheck);
        }

        public static int FindBestMove(SearchContext context, int depth, int ply, int alpha, int beta, bool allowNullMove, bool friendlyKingInCheck)
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

            if (context.BoardState.IsInsufficientMaterial())
            {
                if (!friendlyKingInCheck && !context.BoardState.IsKingChecked(ColorOperations.Invert(context.BoardState.ColorToMove)))
                {
                    context.Statistics.Leafs++;
                    return EvaluationConstants.InsufficientMaterial;
                }
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
            var pvNode = beta - alpha > 1;

            var entry = TranspositionTable.Get(context.BoardState.Hash);
            if (entry.Flags != TranspositionTableEntryFlags.Invalid && entry.IsKeyValid(context.BoardState.Hash))
            {
#if DEBUG
                context.Statistics.TTHits++;
#endif
                if (entry.Flags != TranspositionTableEntryFlags.AlphaScore)
                {
                    var isMoveLegal = context.BoardState.IsMoveLegal(entry.BestMove);
                    if (isMoveLegal)
                    {
                        bestMove = entry.BestMove;
#if DEBUG
                        context.Statistics.TTValidMoves++;
#endif
                    }
#if DEBUG
                    else
                    {
                        context.Statistics.TTInvalidMoves++;
                    }
#endif
                }

                if (entry.Depth >= depth)
                {
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
                            if (!pvNode || entry.Age == context.TranspositionTableEntryAge)
                            {
                                entry.Score = (short)TranspositionTable.TTToRegularScore(entry.Score, ply);
                                return entry.Score;
                            }

                            break;
                        }

                        case TranspositionTableEntryFlags.BetaScore:
                        {
                            if (entry.Score > alpha)
                            {
                                alpha = entry.Score;
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
            }
#endif
            
            if (NullWindowCanBeApplied(context.BoardState, depth, allowNullMove, pvNode, friendlyKingInCheck))
            {
                context.BoardState.MakeNullMove();
                var score = -FindBestMove(context, depth - 1 - SearchConstants.NullWindowDepthReduction, ply + 1, -beta, -beta + 1, false, false);
                context.BoardState.UndoNullMove();

                if (score >= beta)
                {
                    context.Statistics.BetaCutoffs++;
                    return score;
                }
            }
            
            if (IIDCanBeApplied(depth, entry.Flags, bestMove))
            {
                FindBestMove(context, depth - 1 - SearchConstants.IIDDepthReduction, ply, alpha, beta, allowNullMove, friendlyKingInCheck);
                
                var iidEntry = TranspositionTable.Get(context.BoardState.Hash);
                if (iidEntry.IsKeyValid(context.BoardState.Hash))
                {
                    bestMove = iidEntry.BestMove;

#if DEBUG
                    context.Statistics.IIDHits++;
#endif
                }
            }
            
            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            Span<short> moveValues = stackalloc short[SearchConstants.MaxMovesCount];
            var movesCount = 0;
            var movesGenerated = false;

            if (bestMove == Move.Empty)
            {
                movesCount = context.BoardState.GetAvailableMoves(moves);
                MoveOrdering.AssignLoudValues(context.BoardState, moves, moveValues, movesCount, depth, bestMove);
                movesGenerated = true;
            }
            else
            {
                moves[0] = bestMove;
                movesCount = 1;
            }

            var pvs = true;
            var quietValuesGenerated = false;

            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, moveIndex);

                if (movesGenerated && !quietValuesGenerated && moveValues[moveIndex] < 100)
                {
                    MoveOrdering.AssignQuietValues(context.BoardState, moves, moveValues, movesCount, depth, bestMove);
                    MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, moveIndex);
                    quietValuesGenerated = true;
                }

                if (context.MoveRestrictions != null && ply == 0)
                {
                    if (!context.MoveRestrictions.Contains(moves[moveIndex]))
                    {
                        continue;
                    }
                }

                context.BoardState.MakeMove(moves[moveIndex]);

                var score = 0;
                var enemyKingInCheck = context.BoardState.IsKingChecked(context.BoardState.ColorToMove);

                if (pvs)
                {
                    score = -FindBestMove(context, depth - 1, ply + 1, -beta, -alpha, allowNullMove, enemyKingInCheck);
                    pvs = false;
                }
                else
                {
                    var reducedDepth = depth;

                    if (LMRCanBeApplied(depth, friendlyKingInCheck, enemyKingInCheck, moveIndex, moves))
                    {
                        reducedDepth = LMRGetReducedDepth(depth, pvNode);
                    }

                    score = -FindBestMove(context, reducedDepth - 1, ply + 1, -alpha - 1, -alpha, allowNullMove, enemyKingInCheck);
                    if (score > alpha)
                    {
                        score = -FindBestMove(context, depth - 1, ply + 1, -beta, -alpha, allowNullMove, enemyKingInCheck);
                    }
                }

                context.BoardState.UndoMove(moves[moveIndex]);

                if (score > alpha)
                {
                    alpha = score;
                    bestMove = moves[moveIndex];

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

                if (!movesGenerated)
                {
                    movesCount = context.BoardState.GetAvailableMoves(moves);
                    MoveOrdering.AssignLoudValues(context.BoardState, moves, moveValues, movesCount, depth, bestMove);
                    MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, 0);
                    movesGenerated = true;
                }
            }

            // Don't save invalid scores to the transposition table
            if (context.AbortSearch)
            {
                return 0;
            }

            // Don't add invalid move (done after checkmate) to prevent strange behaviors
            if (alpha == -(-EvaluationConstants.Checkmate + ply + 1))
            {
                return alpha;
            }

            // Return draw score or checkmate score as leafs
            if (alpha == -EvaluationConstants.Checkmate + ply + 2)
            {
                if (context.BoardState.IsKingChecked(context.BoardState.ColorToMove))
                {
                    return alpha;
                }
                
                return 0;
            }

            if (entry.Age != context.TranspositionTableEntryAge || entry.Depth <= depth)
            {
                var valueToSave = alpha;
                var entryType = alpha <= originalAlpha ? TranspositionTableEntryFlags.AlphaScore :
                    alpha >= beta ? TranspositionTableEntryFlags.BetaScore :
                    TranspositionTableEntryFlags.ExactScore;

                if (entryType == TranspositionTableEntryFlags.ExactScore)
                {
                    valueToSave = TranspositionTable.RegularToTTScore(alpha, ply);
                }

                TranspositionTable.Add(context.BoardState.Hash, (byte)depth, (short)valueToSave, 
                    (byte)context.TranspositionTableEntryAge, bestMove, entryType);

#if DEBUG
                if (entry.Flags != TranspositionTableEntryFlags.Invalid)
                {
                    context.Statistics.TTReplacements++;
                }

                context.Statistics.TTAddedEntries++;
#endif
            }

            return alpha;
        }

        private static bool NullWindowCanBeApplied(BoardState board, int depth, bool allowNullMove, bool pvNode, bool friendlyKingInCheck)
        {
            return !pvNode && allowNullMove && depth >= SearchConstants.NullWindowMinimalDepth && 
                   board.GetGamePhase() == GamePhase.Opening && !friendlyKingInCheck;
        }

        private static bool IIDCanBeApplied(int depth, TranspositionTableEntryFlags ttEntryType, Move bestMove)
        {
            return ttEntryType == TranspositionTableEntryFlags.Invalid && depth >= SearchConstants.IIDMinimalDepth && bestMove == Move.Empty;
        }

        private static bool LMRCanBeApplied(int depth, bool friendlyKingInCheck, bool enemyKingInCheck, int moveIndex, Span<Move> moves)
        {
            return depth >= SearchConstants.LMRMinimalDepth && moveIndex >= SearchConstants.LMRMovesWithoutReduction &&
                   moves[moveIndex].IsQuiet() && !friendlyKingInCheck && !enemyKingInCheck;
        }

        private static int LMRGetReducedDepth(int depth, bool pvNode)
        {
            return pvNode ? 
                depth - SearchConstants.LMRPvNodeDepthReduction : 
                depth - depth / SearchConstants.LMRNonPvNodeDepthDivisor;
        }
    }
}
