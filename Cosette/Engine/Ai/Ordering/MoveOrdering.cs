using System;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Ordering
{
    public static class MoveOrdering
    {
        public static void AssignValues(BoardState board, Span<Move> moves, Span<short> moveValues, int movesCount, int depth, Move hashOrPvMove)
        {
            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                if (hashOrPvMove == moves[moveIndex])
                {
                    moveValues[moveIndex] = MoveOrderingConstants.HashMove;
                }
                else if (moves[moveIndex].IsQuiet())
                {
                    if (moves[moveIndex].IsPawnNearPromotion(board.ColorToMove))
                    {
                        moveValues[moveIndex] = MoveOrderingConstants.PawnNearPromotion;
                    }
                    else if (KillerHeuristic.KillerMoveExists(moves[moveIndex], board.ColorToMove, depth))
                    {
                        moveValues[moveIndex] = MoveOrderingConstants.KillerMove;
                    }
                    else
                    {
                        moveValues[moveIndex] = HistoryHeuristic.GetHistoryMoveValue(board.ColorToMove, moves[moveIndex].From, moves[moveIndex].To);
                    }
                }
                else if ((moves[moveIndex].Flags & MoveFlags.Kill) != 0)
                {
                    var enemyColor = ColorOperations.Invert(board.ColorToMove);

                    var attackingPiece = moves[moveIndex].PieceType;
                    var capturedPiece = board.GetPiece(enemyColor, moves[moveIndex].To);

                    var attackers = board.GetAttackingPiecesWithColor(board.ColorToMove, moves[moveIndex].To);
                    var defenders = board.GetAttackingPiecesWithColor(enemyColor, moves[moveIndex].To);
                    var seeEvaluation = StaticExchangeEvaluation.Evaluate(attackingPiece, capturedPiece, attackers, defenders);

                    moveValues[moveIndex] = (short)(MoveOrderingConstants.Capture + seeEvaluation);
                }
                else if ((moves[moveIndex].Flags & MoveFlags.EnPassant) != 0)
                {
                    moveValues[moveIndex] = MoveOrderingConstants.EnPassant;
                }
                else if ((int)moves[moveIndex].Flags >= 16)
                {
                    moveValues[moveIndex] = MoveOrderingConstants.Promotion;
                }
            }
        }

        public static void AssignQValues(BoardState board, Span<Move> moves, Span<short> moveValues, int movesCount)
        {
            var enemyColor = ColorOperations.Invert(board.ColorToMove);
            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                if ((moves[moveIndex].Flags & MoveFlags.EnPassant) != 0)
                {
                    moveValues[moveIndex] = MoveOrderingConstants.EnPassant;
                }
                else
                {
                    var attackingPiece = moves[moveIndex].PieceType;
                    var capturedPiece = board.GetPiece(enemyColor, moves[moveIndex].To);

                    var attackers = board.GetAttackingPiecesWithColor(board.ColorToMove, moves[moveIndex].To);
                    var defenders = board.GetAttackingPiecesWithColor(enemyColor, moves[moveIndex].To);
                    var seeEvaluation = StaticExchangeEvaluation.Evaluate(attackingPiece, capturedPiece, attackers, defenders);

                    moveValues[moveIndex] = seeEvaluation;
                }
            }
        }

        public static void SortNextBestMove(Span<Move> moves, Span<short> moveValues, int movesCount, int currentIndex)
        {
            var max = short.MinValue;
            var maxIndex = -1;

            for (var i = currentIndex; i < movesCount; i++)
            {
                if (moveValues[i] > max)
                {
                    max = moveValues[i];
                    maxIndex = i;
                }
            }

            (moves[maxIndex], moves[currentIndex]) = (moves[currentIndex], moves[maxIndex]);
            (moveValues[maxIndex], moveValues[currentIndex]) = (moveValues[currentIndex], moveValues[maxIndex]);
        }
    }
}
