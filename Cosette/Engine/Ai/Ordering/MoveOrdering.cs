using System;
using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Search
{
    public static class MoveOrdering
    {
        public static void AssignValues(BoardState board, Span<Move> moves, Span<int> moveValues, int movesCount, int depth, TranspositionTableEntry entry)
        {
            var enemyColor = ColorOperations.Invert(board.ColorToMove);
            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                if (entry.Type != TranspositionTableEntryType.Invalid && entry.BestMove == moves[moveIndex])
                {
                    moveValues[moveIndex] = MoveOrderingConstants.HashMove;
                }
                else if (moves[moveIndex].IsQuiet())
                {
                    if (KillerHeuristic.KillerMoveExists(moves[moveIndex], board.ColorToMove, depth))
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
                    var attackingPiece = moves[moveIndex].Piece;
                    var capturedPiece = board.GetPiece(enemyColor, moves[moveIndex].To);

                    var attackers = board.GetAttackingPiecesWithColor(board.ColorToMove, moves[moveIndex].To);
                    var defenders = board.GetAttackingPiecesWithColor(enemyColor, moves[moveIndex].To);
                    moveValues[moveIndex] = MoveOrderingConstants.Capture + StaticExchangeEvaluation.Evaluate(attackingPiece, capturedPiece, attackers, defenders);
                }
                else if ((int)moves[moveIndex].Flags >= 16)
                {
                    moveValues[moveIndex] = MoveOrderingConstants.Promotion;
                }
            }
        }

        public static void AssignQValues(BoardState board, Span<Move> moves, Span<int> moveValues, int movesCount)
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
                    moveValues[i] = MoveOrderingConstants.Capture + StaticExchangeEvaluation.Evaluate(attackingPiece, capturedPiece, attackers, defenders);
                }
            }
        }

        public static void SortNextBestMove(Span<Move> moves, Span<int> moveValues, int movesCount, int currentIndex)
        {
            var max = int.MinValue;
            var maxIndex = -1;

            for (var i = currentIndex; i < movesCount; i++)
            {
                if (moveValues[i] > max)
                {
                    max = moveValues[i];
                    maxIndex = i;
                }
            }

            var tempMove = moves[maxIndex];
            moves[maxIndex] = moves[currentIndex];
            moves[currentIndex] = tempMove;

            var tempMoveValue = moveValues[maxIndex];
            moveValues[maxIndex] = moveValues[currentIndex];
            moveValues[currentIndex] = tempMoveValue;
        }
    }
}
