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
                    var pieceType = board.PieceTable[moves[moveIndex].From];
                    if (pieceType == Piece.Pawn && IsPawnNearPromotion(board.ColorToMove, moves[moveIndex].To))
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
                else if (moves[moveIndex].Flags == MoveFlags.EnPassant)
                {
                    moveValues[moveIndex] = MoveOrderingConstants.EnPassant;
                }
                else if (((byte)moves[moveIndex].Flags & MoveFlagFields.Capture) != 0)
                {
                    var enemyColor = ColorOperations.Invert(board.ColorToMove);

                    var attackingPiece = board.PieceTable[moves[moveIndex].From];
                    var capturedPiece = board.PieceTable[moves[moveIndex].To];

                    var attackers = board.GetAttackingPiecesWithColor(board.ColorToMove, moves[moveIndex].To);
                    var defenders = board.GetAttackingPiecesWithColor(enemyColor, moves[moveIndex].To);
                    var seeEvaluation = StaticExchangeEvaluation.Evaluate(attackingPiece, capturedPiece, attackers, defenders);

                    moveValues[moveIndex] = (short)(MoveOrderingConstants.Capture + seeEvaluation);
                }
                else if ((int)moves[moveIndex].Flags >= 16)
                {
                    moveValues[moveIndex] = (short)(MoveOrderingConstants.Promotion + (int)moves[moveIndex].Flags);
                }
            }
        }

        public static void AssignQValues(BoardState board, Span<Move> moves, Span<short> moveValues, int movesCount)
        {
            var enemyColor = ColorOperations.Invert(board.ColorToMove);
            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                if (moves[moveIndex].Flags == MoveFlags.EnPassant)
                {
                    moveValues[moveIndex] = MoveOrderingConstants.EnPassant;
                }
                else
                {
                    var attackingPiece = board.PieceTable[moves[moveIndex].From];
                    var capturedPiece = board.PieceTable[moves[moveIndex].To];

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


        private static bool IsPawnNearPromotion(int color, byte to)
        {
            if (color == Color.White && to >= 40 || color == Color.Black && to <= 23)
            {
                return true;
            }

            return false;
        }
    }
}
