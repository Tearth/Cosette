using System;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class BishopOperator
    {
        public static int GetLoudMoves(BoardState boardState, Span<Move> moves, int offset)
        {
            var color = boardState.ColorToMove;
            var enemyColor = ColorOperations.Invert(color);
            var bishops = boardState.Pieces[color][Piece.Bishop];

            while (bishops != 0)
            {
                var piece = BitOperations.GetLsb(bishops);
                bishops = BitOperations.PopLsb(bishops);

                var from = BitOperations.BitScan(piece);
                var availableMoves = BishopMovesGenerator.GetMoves(boardState.OccupancySummary, from) & boardState.Occupancy[enemyColor];

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);
                    availableMoves = BitOperations.PopLsb(availableMoves);

                    moves[offset++] = new Move(from, fieldIndex, MoveFlags.Capture);
                }
            }

            return offset;
        }

        public static int GetQuietMoves(BoardState boardState, Span<Move> moves, int offset)
        {
            var color = boardState.ColorToMove;
            var enemyColor = ColorOperations.Invert(color);
            var bishops = boardState.Pieces[color][Piece.Bishop];

            while (bishops != 0)
            {
                var piece = BitOperations.GetLsb(bishops);
                bishops = BitOperations.PopLsb(bishops);

                var from = BitOperations.BitScan(piece);
                var availableMoves = BishopMovesGenerator.GetMoves(boardState.OccupancySummary, from) & ~boardState.OccupancySummary;

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);
                    availableMoves = BitOperations.PopLsb(availableMoves);

                    moves[offset++] = new Move(from, fieldIndex, MoveFlags.Quiet);
                }
            }

            return offset;
        }

        public static int GetAvailableCaptureMoves(BoardState boardState, Span<Move> moves, int offset)
        {
            var color = boardState.ColorToMove;
            var enemyColor = ColorOperations.Invert(color);
            var bishops = boardState.Pieces[color][Piece.Bishop];

            while (bishops != 0)
            {
                var piece = BitOperations.GetLsb(bishops);
                bishops = BitOperations.PopLsb(bishops);

                var from = BitOperations.BitScan(piece);
                var availableMoves = BishopMovesGenerator.GetMoves(boardState.OccupancySummary, from) & boardState.Occupancy[enemyColor];

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);
                    availableMoves = BitOperations.PopLsb(availableMoves);

                    moves[offset++] = new Move(from, fieldIndex, MoveFlags.Capture);
                }
            }

            return offset;
        }

        public static (int, int) GetMobility(BoardState boardState, int color, ref ulong fieldsAttackedByColor)
        {
            var centerMobility = 0;
            var outsideMobility = 0;

            var bishops = boardState.Pieces[color][Piece.Bishop];

            while (bishops != 0)
            {
                var piece = BitOperations.GetLsb(bishops);
                bishops = BitOperations.PopLsb(bishops);

                var from = BitOperations.BitScan(piece);
                var availableMoves = BishopMovesGenerator.GetMoves(boardState.OccupancySummary, from);

                centerMobility += (int)BitOperations.Count(availableMoves & EvaluationConstants.Center);
                outsideMobility += (int)BitOperations.Count(availableMoves & EvaluationConstants.Outside);

                fieldsAttackedByColor |= availableMoves;
            }

            return (centerMobility, outsideMobility);
        }

        public static bool IsMoveLegal(BoardState boardState, Move move)
        {
            var enemyColor = ColorOperations.Invert(boardState.ColorToMove);
            var availableMoves = BishopMovesGenerator.GetMoves(boardState.OccupancySummary, move.From);
            var toField = 1ul << move.To;

            if (move.Flags == MoveFlags.Quiet && (availableMoves & toField) != 0 && (boardState.OccupancySummary & toField) == 0)
            {
                return true;
            }

            if (move.Flags == MoveFlags.Capture && (availableMoves & toField) != 0 && (boardState.Occupancy[enemyColor] & toField) != 0)
            {
                return true;
            }

            return false;
        }
    }
}