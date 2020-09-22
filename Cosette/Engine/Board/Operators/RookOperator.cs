using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class RookOperator
    {
        public static int GetAvailableMoves(BoardState boardState, int color, Span<Move> moves, int offset)
        {
            var enemyColor = ColorOperations.Invert(color);
            var rooks = boardState.Pieces[color][Piece.Rook];

            while (rooks != 0)
            {
                var piece = BitOperations.GetLsb(rooks);
                rooks = BitOperations.PopLsb(rooks);

                var from = BitOperations.BitScan(piece);
                var availableMoves = RookMovesGenerator.GetMoves(boardState.OccupancySummary, from) & ~boardState.Occupancy[color];

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);
                    availableMoves = BitOperations.PopLsb(availableMoves);

                    var flags = (field & boardState.Occupancy[enemyColor]) != 0 ? MoveFlags.Kill : MoveFlags.None;
                    moves[offset++] = new Move(from, fieldIndex, Piece.Rook, flags);
                }
            }

            return offset;
        }

        public static int GetAvailableQMoves(BoardState boardState, int color, Span<Move> moves, int offset)
        {
            var enemyColor = ColorOperations.Invert(color);
            var rooks = boardState.Pieces[color][Piece.Rook];

            while (rooks != 0)
            {
                var piece = BitOperations.GetLsb(rooks);
                rooks = BitOperations.PopLsb(rooks);

                var from = BitOperations.BitScan(piece);
                var availableMoves = RookMovesGenerator.GetMoves(boardState.OccupancySummary, from) & boardState.Occupancy[enemyColor];

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);
                    availableMoves = BitOperations.PopLsb(availableMoves);

                    moves[offset++] = new Move(from, fieldIndex, Piece.Rook, MoveFlags.Kill);
                }
            }

            return offset;
        }

        public static int GetMobility(BoardState boardState, int color)
        {
            var mobility = 0;
            var rooks = boardState.Pieces[color][Piece.Rook];

            while (rooks != 0)
            {
                var piece = BitOperations.GetLsb(rooks);
                rooks = BitOperations.PopLsb(rooks);

                var from = BitOperations.BitScan(piece);
                var availableMoves = RookMovesGenerator.GetMoves(boardState.OccupancySummary, from);
                mobility += (int)BitOperations.Count(availableMoves);
            }

            return mobility;
        }
    }
}
