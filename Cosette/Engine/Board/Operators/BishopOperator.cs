using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class BishopOperator
    {
        public static int GetAvailableMoves(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            var enemyColor = ColorOperations.Invert(color);
            var bishops = boardState.Pieces[(int) color][(int) Piece.Bishop];

            while (bishops != 0)
            {
                var piece = BitOperations.GetLsb(bishops);
                bishops = BitOperations.PopLsb(bishops);

                var from = BitOperations.BitScan(piece);
                var availableMoves = BishopMovesGenerator.GetMoves(boardState.OccupancySummary, from) & ~boardState.Occupancy[(int)color];

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    availableMoves = BitOperations.PopLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);

                    var flags = (field & boardState.Occupancy[(int)enemyColor]) != 0 ? MoveFlags.Kill : MoveFlags.None;
                    moves[offset++] = new Move(from, fieldIndex, Piece.Bishop, flags);
                }
            }

            return offset;
        }

        public static int GetAvailableQuiescenceMoves(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            var enemyColor = ColorOperations.Invert(color);
            var bishops = boardState.Pieces[(int)color][(int)Piece.Bishop];

            while (bishops != 0)
            {
                var piece = BitOperations.GetLsb(bishops);
                bishops = BitOperations.PopLsb(bishops);

                var from = BitOperations.BitScan(piece);
                var availableMoves = BishopMovesGenerator.GetMoves(boardState.OccupancySummary, from) & boardState.Occupancy[(int)enemyColor];

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    availableMoves = BitOperations.PopLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);

                    moves[offset++] = new Move(from, fieldIndex, Piece.Bishop, MoveFlags.Kill);
                }
            }

            return offset;
        }

        public static int GetMobility(BoardState boardState, Color color)
        {
            var mobility = 0;
            var bishops = boardState.Pieces[(int)color][(int)Piece.Bishop];

            while (bishops != 0)
            {
                var piece = BitOperations.GetLsb(bishops);
                bishops = BitOperations.PopLsb(bishops);

                var from = BitOperations.BitScan(piece);
                var availableMoves = BishopMovesGenerator.GetMoves(boardState.OccupancySummary, from);
                mobility += (int) BitOperations.Count(availableMoves);
            }

            return mobility;
        }
    }
}