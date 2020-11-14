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

            if (piece == 0)
            {
                return offset;
            }

            var from = BitOperations.BitScan(piece);
            var availableMoves = KingMovesGenerator.GetMoves(from) & ~boardState.Occupancy[color];

            while (availableMoves != 0)
            {
                var field = BitOperations.GetLsb(availableMoves);
                var fieldIndex = BitOperations.BitScan(field);
                availableMoves = BitOperations.PopLsb(availableMoves);

                var flags = (field & boardState.Occupancy[enemyColor]) != 0 ? MoveFlags.Capture : MoveFlags.Quiet;
                moves[offset++] = new Move(from, fieldIndex, flags);
            }

            if (color == Color.White)
            {
                if ((boardState.Castling & Castling.WhiteShort) != 0 && (boardState.OccupancySummary & 6) == 0)
                {
                    if (!boardState.IsFieldAttacked(color, 1) && !boardState.IsFieldAttacked(color, 2) && !boardState.IsFieldAttacked(color, 3))
                    {
                        moves[offset++] = new Move(3, 1, MoveFlags.KingCastle);
                    }
                }
                
                if ((boardState.Castling & Castling.WhiteLong) != 0 && (boardState.OccupancySummary & 112) == 0)
                {
                    if (!boardState.IsFieldAttacked(color, 3) && !boardState.IsFieldAttacked(color, 4) && !boardState.IsFieldAttacked(color, 5))
                    {
                        moves[offset++] = new Move(3, 5, MoveFlags.QueenCastle);
                    }
                }
            }
            else
            {
                if ((boardState.Castling & Castling.BlackShort) != 0 && (boardState.OccupancySummary & 432345564227567616) == 0)
                {
                    if (!boardState.IsFieldAttacked(color, 57) && !boardState.IsFieldAttacked(color, 58) && !boardState.IsFieldAttacked(color, 59))
                    {
                        moves[offset++] = new Move(59, 57, MoveFlags.KingCastle);
                    }
                }
                
                if ((boardState.Castling & Castling.BlackLong) != 0 && (boardState.OccupancySummary & 8070450532247928832) == 0)
                {
                    if (!boardState.IsFieldAttacked(color, 59) && !boardState.IsFieldAttacked(color, 60) && !boardState.IsFieldAttacked(color, 61))
                    {
                        moves[offset++] = new Move(59, 61, MoveFlags.QueenCastle);
                    }
                }
            }

            return offset;
        }

        public static int GetAvailableQMoves(BoardState boardState, int color, Span<Move> moves, int offset)
        {
            var enemyColor = ColorOperations.Invert(color);
            var piece = boardState.Pieces[color][Piece.King];

            if (piece == 0)
            {
                return offset;
            }

            var from = BitOperations.BitScan(piece);
            var availableMoves = KingMovesGenerator.GetMoves(from) & boardState.Occupancy[enemyColor];

            while (availableMoves != 0)
            {
                var field = BitOperations.GetLsb(availableMoves);
                var fieldIndex = BitOperations.BitScan(field);
                availableMoves = BitOperations.PopLsb(availableMoves);

                moves[offset++] = new Move(from, fieldIndex, MoveFlags.Capture);
            }

            return offset;
        }
    }
}