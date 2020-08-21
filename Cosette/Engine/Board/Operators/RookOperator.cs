using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class RookOperator
    {
        public static int GetAvailableMoves(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            var friendlyOccupancy = boardState.Occupancy[(int)color];
            var enemyOccupancy = boardState.Occupancy[(int)ColorOperations.Invert(color)];
            var rooks = boardState.Pieces[(int)color][(int)Piece.Rook];

            while (rooks != 0)
            {
                var piece = BitOperations.GetLsb(rooks);
                rooks = BitOperations.PopLsb(rooks);

                var from = BitOperations.BitScan(piece);
                var availableMoves = RookMovesGenerator.GetMoves(boardState.OccupancySummary, from) & ~friendlyOccupancy;

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    availableMoves = BitOperations.PopLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);

                    moves[offset++] = new Move(from, fieldIndex, Piece.Rook, (field & enemyOccupancy) != 0 ? MoveFlags.Kill : MoveFlags.None);
                }
            }

            return offset;
        }
    }
}
