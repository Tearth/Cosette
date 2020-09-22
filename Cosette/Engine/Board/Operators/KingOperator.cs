using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class KingOperator
    {
        public static int GetAvailableMoves(BoardState boardState, int color, Span<Move> moves, int offset)
        {
            var enemyColor = ColorOperations.Invert(color);
            var piece = boardState.Pieces[color][Piece.King];

            var from = BitOperations.BitScan(piece);
            var availableMoves = KingMovesGenerator.GetMoves(from) & ~boardState.Occupancy[color];

            while (availableMoves != 0)
            {
                var field = BitOperations.GetLsb(availableMoves);
                availableMoves = BitOperations.PopLsb(availableMoves);
                var fieldIndex = BitOperations.BitScan(field);

                var flags = (field & boardState.Occupancy[enemyColor]) != 0 ? MoveFlags.Kill : MoveFlags.None;
                moves[offset++] = new Move(from, fieldIndex, Piece.King, flags);
            }

            if (color == Color.White)
            {
                if ((boardState.Castling & Castling.WhiteShort) != 0 && (boardState.OccupancySummary & 6) == 0)
                {
                    if (!boardState.IsFieldAttacked(color, 1) && !boardState.IsFieldAttacked(color, 2) && !boardState.IsFieldAttacked(color, 3))
                    {
                        moves[offset++] = new Move(3, 1, Piece.King, MoveFlags.Castling);
                    }
                }
                
                if ((boardState.Castling & Castling.WhiteLong) != 0 && (boardState.OccupancySummary & 112) == 0)
                {
                    if (!boardState.IsFieldAttacked(color, 3) && !boardState.IsFieldAttacked(color, 4) && !boardState.IsFieldAttacked(color, 5))
                    {
                        moves[offset++] = new Move(3, 5, Piece.King, MoveFlags.Castling);
                    }
                }
            }
            else
            {
                if ((boardState.Castling & Castling.BlackShort) != 0 && (boardState.OccupancySummary & 432345564227567616) == 0)
                {
                    if (!boardState.IsFieldAttacked(color, 57) && !boardState.IsFieldAttacked(color, 58) && !boardState.IsFieldAttacked(color, 59))
                    {
                        moves[offset++] = new Move(59, 57, Piece.King, MoveFlags.Castling);
                    }
                }
                
                if ((boardState.Castling & Castling.BlackLong) != 0 && (boardState.OccupancySummary & 8070450532247928832) == 0)
                {
                    if (!boardState.IsFieldAttacked(color, 59) && !boardState.IsFieldAttacked(color, 60) && !boardState.IsFieldAttacked(color, 61))
                    {
                        moves[offset++] = new Move(59, 61, Piece.King, MoveFlags.Castling);
                    }
                }
            }

            return offset;
        }

        public static int GetAvailableQMoves(BoardState boardState, int color, Span<Move> moves, int offset)
        {
            var enemyColor = ColorOperations.Invert(color);
            var piece = boardState.Pieces[color][Piece.King];

            var from = BitOperations.BitScan(piece);
            var availableMoves = KingMovesGenerator.GetMoves(from) & boardState.Occupancy[enemyColor];

            while (availableMoves != 0)
            {
                var field = BitOperations.GetLsb(availableMoves);
                availableMoves = BitOperations.PopLsb(availableMoves);
                var fieldIndex = BitOperations.BitScan(field);

                moves[offset++] = new Move(from, fieldIndex, Piece.King, MoveFlags.Kill);
            }

            return offset;
        }
    }
}