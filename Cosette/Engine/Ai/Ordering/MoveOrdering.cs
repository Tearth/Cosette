using System;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Ordering
{
    public static class MoveOrdering
    {
        public static void AssignValues(BoardState board, Span<Move> moves, Span<short> moveValues, int movesCount, byte depth, TranspositionTableEntry entry)
        {
            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                if (entry.BestMove == moves[moveIndex])
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

                    var attackingPiece = moves[moveIndex].Piece;
                    var capturedPiece = board.GetPiece(enemyColor, moves[moveIndex].To);

                    var attackers = board.GetAttackingPiecesWithColor(board.ColorToMove, moves[moveIndex].To);
                    var defenders = board.GetAttackingPiecesWithColor(enemyColor, moves[moveIndex].To);
                    var seeEvaluation = StaticExchangeEvaluation.Evaluate(attackingPiece, capturedPiece, attackers, defenders);

                    moveValues[moveIndex] = (short)(MoveOrderingConstants.Capture + seeEvaluation);
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
            for (var i = 0; i < movesCount; i++)
            {
                if ((moves[i].Flags & MoveFlags.EnPassant) != 0)
                {
                    moveValues[i] = MoveOrderingConstants.Capture;
                }
                else
                {
                    var attackingPiece = moves[i].Piece;
                    var capturedPiece = board.GetPiece(enemyColor, moves[i].To);

                    var attackers = board.GetAttackingPiecesWithColor(board.ColorToMove, moves[i].To);
                    var defenders = board.GetAttackingPiecesWithColor(enemyColor, moves[i].To);
                    var seeEvaluation = StaticExchangeEvaluation.Evaluate(attackingPiece, capturedPiece, attackers, defenders);

                    moveValues[i] = (short)(MoveOrderingConstants.Capture + seeEvaluation);
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
