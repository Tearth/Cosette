using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class QueenOperator
    {
        public static int GetAvailableMoves(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            var friendlyOccupancy = boardState.ColorOccupancy[(int)color];
            var enemyOccupancy = boardState.ColorOccupancy[(int)ColorOperations.Invert(color)];
            var queens = color == Color.White ? boardState.WhitePieces[(int)Piece.Queen] : boardState.BlackPieces[(int)Piece.Queen];

            while (queens != 0)
            {
                var piece = BitOperations.GetLsb(queens);
                queens = BitOperations.PopLsb(queens);

                var from = BitOperations.BitScan(piece);
                var availableMoves = QueenMovesGenerator.GetMoves(boardState.Occupancy, from) & ~friendlyOccupancy;

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    availableMoves = BitOperations.PopLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);

                    moves[offset++] = new Move(from, fieldIndex, Piece.Queen, (field & enemyOccupancy) != 0 ? MoveFlags.Kill : MoveFlags.None);
                }
            }

            return offset;
        }
    }
}