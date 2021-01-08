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
            return FindBestMove(context, depth, ply, alpha, beta, true, friendlyKingInCheck, 0);
        }

        public static int FindBestMove(SearchContext context, int depth, int ply, int alpha, int beta, bool allowNullMove, bool friendlyKingInCheck, int extensionsCount)
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
            var pvNode = beta - alpha > 1;

            var entry = TranspositionTable.Get(context.BoardState.Hash);
            var hashMove = Move.Empty;

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
                        hashMove = entry.BestMove;
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
                            if (!pvNode)
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

            if (StaticNullMoveCanBeApplied(depth, friendlyKingInCheck, pvNode, beta))
            {
                var fastEvaluation = Evaluation.FastEvaluate(context.BoardState);
                var margin = SearchConstants.StaticNullMoveBaseMargin + depth * SearchConstants.StaticNullMoveMarginMultiplier;
                var score = fastEvaluation - margin;

                if (score >= beta)
                {
                    context.Statistics.FutilityPrunes++;
                    return score;
                }
            }

            if (NullMoveCanBeApplied(context.BoardState, depth, allowNullMove, pvNode, friendlyKingInCheck))
            {
                context.BoardState.MakeNullMove();
                var score = -FindBestMove(context, depth - 1 - NullMoveGetReduction(depth), ply + 1, -beta, -beta + 1, false, false, extensionsCount);
                context.BoardState.UndoNullMove();

                if (score >= beta)
                {
                    context.Statistics.NullMovePrunes++;
                    return score;
                }
            }
            
            if (IIDCanBeApplied(depth, entry.Flags, hashMove))
            {
                FindBestMove(context, depth - 1 - SearchConstants.IIDDepthReduction, ply, alpha, beta, allowNullMove, friendlyKingInCheck, extensionsCount);
                
                var iidEntry = TranspositionTable.Get(context.BoardState.Hash);
                if (iidEntry.IsKeyValid(context.BoardState.Hash))
                {
                    hashMove = iidEntry.BestMove;

#if DEBUG
                    context.Statistics.IIDHits++;
#endif
                }
            }
            
            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            Span<short> moveValues = stackalloc short[SearchConstants.MaxMovesCount];
            var bestMove = Move.Empty;
            var movesCount = 0;

            var loudMovesGenerated = false;
            var quietMovesGenerated = false;

            var evasionMask = ulong.MaxValue;
            if (friendlyKingInCheck && !context.BoardState.IsKingChecked(ColorOperations.Invert(context.BoardState.ColorToMove)))
            {
                var kingField = context.BoardState.Pieces[context.BoardState.ColorToMove][Piece.King];
                var kingFieldIndex = BitOperations.BitScan(kingField);

                evasionMask = KnightMovesGenerator.GetMoves(kingFieldIndex) | 
                              QueenMovesGenerator.GetMoves(context.BoardState.OccupancySummary, kingFieldIndex);
            }

            if (hashMove == Move.Empty)
            {
                movesCount = context.BoardState.GetLoudMoves(moves, 0, evasionMask);
                MoveOrdering.AssignLoudValues(context.BoardState, moves, moveValues, movesCount, depth, bestMove);
                loudMovesGenerated = true;

                context.Statistics.LoudMovesGenerated++;

                if (movesCount == 0)
                {
                    movesCount = context.BoardState.GetQuietMoves(moves, 0, evasionMask);
                    MoveOrdering.AssignQuietValues(context.BoardState, moves, moveValues, 0, movesCount, depth);
                    quietMovesGenerated = true;

                    context.Statistics.QuietMovesGenerated++;
                }
            }
            else
            {
                moves[0] = hashMove;
                moveValues[0] = MoveOrderingConstants.HashMove;
                movesCount = 1;
            }

            var pvs = true;
            var bestScore = 0;

            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, moveIndex);

                if (loudMovesGenerated && moves[moveIndex] == hashMove)
                {
                    goto postLoopOperations;
                }

                if (loudMovesGenerated && !quietMovesGenerated && moveValues[moveIndex] < 100)
                {
                    var loudMovesCount = movesCount;

                    movesCount = context.BoardState.GetQuietMoves(moves, movesCount, evasionMask);
                    MoveOrdering.AssignQuietValues(context.BoardState, moves, moveValues, loudMovesCount, movesCount, depth);
                    MoveOrdering.SortNextBestMove(moves, moveValues, movesCount, moveIndex);
                    quietMovesGenerated = true;

                    context.Statistics.QuietMovesGenerated++;

                    if (moves[moveIndex] == hashMove)
                    {
                        goto postLoopOperations;
                    }
                }

                if (context.MoveRestrictions != null && ply == 0)
                {
                    if (!context.MoveRestrictions.Contains(moves[moveIndex]))
                    {
                        continue;
                    }
                }

                context.BoardState.MakeMove(moves[moveIndex]);
                
                var enemyKingInCheck = context.BoardState.IsKingChecked(context.BoardState.ColorToMove);
                var extension = GetExtensions(depth, extensionsCount, enemyKingInCheck);

#if DEBUG
                context.Statistics.Extensions += extension;
#endif

                if (pvs)
                {
                    bestScore = -FindBestMove(context, depth - 1 + extension, ply + 1, -beta, -alpha, allowNullMove, enemyKingInCheck, extensionsCount + extension);
                    pvs = false;
                }
                else
                {
                    var lmrReduction = 0;
                    if (LMRCanBeApplied(context, depth, friendlyKingInCheck, enemyKingInCheck, moveIndex, moves))
                    {
                        lmrReduction = LMRGetReduction(pvNode, moveIndex);
                    }

                    var score = -FindBestMove(context, depth - lmrReduction - 1 + extension, ply + 1, -alpha - 1, -alpha, allowNullMove, enemyKingInCheck, extensionsCount + extension);
                    if (score > alpha)
                    {
                        if (pvNode)
                        {
                            score = -FindBestMove(context, depth - 1 + extension, ply + 1, -beta, -alpha, allowNullMove, enemyKingInCheck, extensionsCount + extension);
                        }
                        else
                        {
                            if (lmrReduction != 0)
                            {
                                score = -FindBestMove(context, depth - 1 + extension, ply + 1, -beta, -alpha, allowNullMove, enemyKingInCheck, extensionsCount + extension);
                            }
                        }
                    }

                    if (score > bestScore)
                    {
                        bestScore = score;
                    }
                }

                context.BoardState.UndoMove(moves[moveIndex]);

                if (bestScore > alpha)
                {
                    alpha = bestScore;
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

                postLoopOperations:
                if (!loudMovesGenerated)
                {
                    movesCount = context.BoardState.GetLoudMoves(moves, 0, evasionMask);
                    MoveOrdering.AssignLoudValues(context.BoardState, moves, moveValues, movesCount, depth, bestMove);
                    moveIndex = -1;
                    loudMovesGenerated = true;

                    context.Statistics.LoudMovesGenerated++;

                    if (movesCount == 0)
                    {
                        movesCount = context.BoardState.GetQuietMoves(moves, 0, evasionMask);
                        MoveOrdering.AssignQuietValues(context.BoardState, moves, moveValues, 0, movesCount, depth);
                        quietMovesGenerated = true;

                        context.Statistics.QuietMovesGenerated++;
                    }
                }

                if (!quietMovesGenerated && moveIndex == movesCount - 1)
                {
                    var loudMovesCount = movesCount;

                    movesCount = context.BoardState.GetQuietMoves(moves, movesCount, evasionMask);
                    MoveOrdering.AssignQuietValues(context.BoardState, moves, moveValues, loudMovesCount, movesCount, depth);
                    quietMovesGenerated = true;

                    context.Statistics.QuietMovesGenerated++;
                }
            }

            // Don't save invalid scores to the transposition table
            if (context.AbortSearch)
            {
                return 0;
            }

            // Don't add invalid move (done after checkmate) to prevent strange behaviors
            if (bestScore == -(-EvaluationConstants.Checkmate + ply + 1))
            {
                return bestScore;
            }

            // Return draw score or checkmate score as leafs
            if (bestScore == -EvaluationConstants.Checkmate + ply + 2)
            {
                if (friendlyKingInCheck)
                {
                    return bestScore;
                }
                
                return 0;
            }

            if (entry.Flags == TranspositionTableEntryFlags.Invalid || alpha != originalAlpha)
            {
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

                    TranspositionTable.Add(context.BoardState.Hash, new TranspositionTableEntry(
                        context.BoardState.Hash,
                        (short)valueToSave, bestMove,
                        (byte)depth, entryType,
                        (byte)context.TranspositionTableEntryAge)
                    );

#if DEBUG
                    if (entry.Flags != TranspositionTableEntryFlags.Invalid)
                    {
                        context.Statistics.TTReplacements++;
                    }

                    context.Statistics.TTAddedEntries++;
#endif
                }
            }

            return bestScore;
        }

        private static bool StaticNullMoveCanBeApplied(int depth, bool friendlyKingInCheck, bool pvNode, int beta)
        {
            return !pvNode && depth <= SearchConstants.StaticNullMoveMaximalDepth && !friendlyKingInCheck && 
                   !IterativeDeepening.IsScoreNearCheckmate(beta);
        }

        private static bool NullMoveCanBeApplied(BoardState board, int depth, bool allowNullMove, bool pvNode, bool friendlyKingInCheck)
        {
            return !pvNode && allowNullMove && depth >= SearchConstants.NullMoveMinimalDepth && 
                   board.GetGamePhase() == GamePhase.Opening && !friendlyKingInCheck;
        }

        private static int NullMoveGetReduction(int depth)
        {
            return SearchConstants.NullMoveBaseDepthReduction + depth / SearchConstants.NullMoveDepthDivider;
        }

        private static bool IIDCanBeApplied(int depth, TranspositionTableEntryFlags ttEntryType, Move bestMove)
        {
            return ttEntryType == TranspositionTableEntryFlags.Invalid && depth >= SearchConstants.IIDMinimalDepth && bestMove == Move.Empty;
        }

        private static bool LMRCanBeApplied(SearchContext context, int depth, bool friendlyKingInCheck, bool enemyKingInCheck, int moveIndex, Span<Move> moves)
        {
            if (depth >= SearchConstants.LMRMinimalDepth && moveIndex >= SearchConstants.LMRMovesWithoutReduction &&
                moves[moveIndex].IsQuiet() && !friendlyKingInCheck && !enemyKingInCheck)
            {
                var enemyColor = ColorOperations.Invert(context.BoardState.ColorToMove);
                var piece = context.BoardState.PieceTable[moves[moveIndex].To];

                if (piece == Piece.Pawn && context.BoardState.IsFieldPassing(enemyColor, moves[moveIndex].To))
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        private static int LMRGetReduction(bool pvNode, int moveIndex)
        {
            var r = SearchConstants.LMRBaseReduction + moveIndex / SearchConstants.LMRMoveIndexDivider;
            return Math.Min(pvNode ? SearchConstants.LMRPvNodeMaxReduction : SearchConstants.LMRNonPvNodeMaxReduction, r);
        }

        private static int GetExtensions(int depth, int extensionsCount, bool enemyKingInCheck)
        {
            if (depth == 1 && extensionsCount == 0 && enemyKingInCheck)
            {
                return 1;
            }

            return 0;
        }
    }
}
