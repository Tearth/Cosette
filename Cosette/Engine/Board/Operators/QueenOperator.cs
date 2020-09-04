using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class QueenOperator
    {
        public static int GetAvailableMoves(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            var enemyColor = ColorOperations.Invert(color);
            var queens = boardState.Pieces[(int)color][(int)Piece.Queen];

            while (queens != 0)
            {
                var piece = BitOperations.GetLsb(queens);
                queens = BitOperations.PopLsb(queens);

                var from = BitOperations.BitScan(piece);
                var availableMoves = QueenMovesGenerator.GetMoves(boardState.OccupancySummary, from) & ~boardState.Occupancy[(int)color];

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    availableMoves = BitOperations.PopLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);

                    var flags = (field & boardState.Occupancy[(int)enemyColor]) != 0 ? MoveFlags.Kill : MoveFlags.None;
                    moves[offset++] = new Move(from, fieldIndex, Piece.Queen, flags);
                }
            }

            return offset;
        }

        public static int GetAvailableQuiescenceMoves(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            var enemyColor = ColorOperations.Invert(color);
            var queens = boardState.Pieces[(int)color][(int)Piece.Queen];

            while (queens != 0)
            {
                var piece = BitOperations.GetLsb(queens);
                queens = BitOperations.PopLsb(queens);

                var from = BitOperations.BitScan(piece);
                var availableMoves = QueenMovesGenerator.GetMoves(boardState.OccupancySummary, from) & boardState.Occupancy[(int)enemyColor];

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    availableMoves = BitOperations.PopLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);

                    moves[offset++] = new Move(from, fieldIndex, Piece.Queen, MoveFlags.Kill);
                }
            }

            return offset;
        }

        public static int GetMobility(BoardState boardState, Color color)
        {
            var mobility = 0;
            var queens = boardState.Pieces[(int)color][(int)Piece.Queen];

            while (queens != 0)
            {
                var piece = BitOperations.GetLsb(queens);
                queens = BitOperations.PopLsb(queens);

                var from = BitOperations.BitScan(piece);
                var availableMoves = QueenMovesGenerator.GetMoves(boardState.OccupancySummary, from) & ~boardState.Occupancy[(int)color];
                mobility += (int)BitOperations.Count(availableMoves);
            }

            return mobility;
        }
    }
}