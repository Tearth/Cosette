using System;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Ordering
{
    public static class MoveOrdering
    {
        public static void AssignLoudValues(BoardState board, Span<Move> moves, Span<short> moveValues, int movesCount, int depth, Move hashOrPvMove)
        {
            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                if (hashOrPvMove == moves[moveIndex])
                {
                    moveValues[moveIndex] = MoveOrderingConstants.HashMove;
                }
                else if (moves[moveIndex].IsEnPassant())
                {
                    moveValues[moveIndex] = MoveOrderingConstants.EnPassant;
                }
                else if (moves[moveIndex].IsPromotion())
                {
                    moveValues[moveIndex] = (short)(MoveOrderingConstants.Promotion + (int)moves[moveIndex].Flags);
                }
                else if (moves[moveIndex].IsCapture())
                {
                    var enemyColor = ColorOperations.Invert(board.ColorToMove);

                    var attackingPiece = board.PieceTable[moves[moveIndex].From];
                    var capturedPiece = board.PieceTable[moves[moveIndex].To];

                    var attackers = board.GetAttackingPiecesWithColor(board.ColorToMove, moves[moveIndex].To);
                    var defenders = board.GetAttackingPiecesWithColor(enemyColor, moves[moveIndex].To);
                    var seeEvaluation = StaticExchangeEvaluation.Evaluate(attackingPiece, capturedPiece, attackers, defenders);

                    moveValues[moveIndex] = (short)(MoveOrderingConstants.Capture + seeEvaluation);
                }
                else if (moves[moveIndex].IsCastling())
                {
                    moveValues[moveIndex] = MoveOrderingConstants.Castling;
                }
                else
                {
                    moveValues[moveIndex] = MoveOrderingConstants.PawnNearPromotion;
                }
            }
        }

        public static void AssignQuietValues(BoardState board, Span<Move> moves, Span<short> moveValues, int startIndex, int movesCount, int ply)
        {
            for (var moveIndex = startIndex; moveIndex < movesCount; moveIndex++)
            {
                if (KillerHeuristic.KillerMoveExists(moves[moveIndex], board.ColorToMove, ply))
                {
                    moveValues[moveIndex] = MoveOrderingConstants.KillerMove;
                }
                else
                {
                    moveValues[moveIndex] = HistoryHeuristic.GetMoveValue(board.ColorToMove, board.PieceTable[moves[moveIndex].From], moves[moveIndex].To, MoveOrderingConstants.HistoryHeuristicMaxScore);
                }
            }
        }

        public static void AssignQValues(BoardState board, Span<Move> moves, Span<short> moveValues, int movesCount)
        {
            var enemyColor = ColorOperations.Invert(board.ColorToMove);
            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                if (moves[moveIndex].IsEnPassant())
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
            if (movesCount <= 1)
            {
                return;
            }

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
