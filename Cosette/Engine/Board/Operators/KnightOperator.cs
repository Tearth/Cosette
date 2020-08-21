using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class KnightOperator
    {
        public static int GetAvailableMoves(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            var friendlyOccupancy = boardState.Occupancy[(int)color];
            var enemyOccupancy = boardState.Occupancy[(int)ColorOperations.Invert(color)];
            var knights = boardState.Pieces[(int)color][(int)Piece.Knight];

            while (knights != 0)
            {
                var piece = BitOperations.GetLsb(knights);
                knights = BitOperations.PopLsb(knights);

                var from = BitOperations.BitScan(piece);
                var availableMoves = KnightMovesGenerator.GetMoves(from) & ~friendlyOccupancy;

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    availableMoves = BitOperations.PopLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);

                    moves[offset++] = new Move(from, fieldIndex, Piece.Knight, (field & enemyOccupancy) != 0 ? MoveFlags.Kill : MoveFlags.None);
                }
            }

            return offset;
        }
    }
}