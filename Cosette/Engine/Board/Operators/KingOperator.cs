using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class KingOperator
    {
        public static int GetAvailableMoves(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            var friendlyOccupancy = color == Color.White ? boardState.WhiteOccupancy : boardState.BlackOccupancy;
            var enemyOccupancy = color == Color.White ? boardState.BlackOccupancy : boardState.WhiteOccupancy;
            var kings = color == Color.White ? boardState.WhitePieces[(int)Piece.King] : boardState.BlackPieces[(int)Piece.King];

            while (kings != 0)
            {
                var piece = BitOperations.GetLsb(kings);
                kings = BitOperations.PopLsb(kings);

                var from = BitOperations.BitScan(piece);
                var availableMoves = KingMovesGenerator.GetMoves(from) & ~friendlyOccupancy;

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    availableMoves = BitOperations.PopLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);

                    moves[offset++] = new Move(from, fieldIndex, Piece.King, (field & enemyOccupancy) != 0 ? MoveFlags.Kill : MoveFlags.None);
                }
            }

            return offset;
        }
    }
}