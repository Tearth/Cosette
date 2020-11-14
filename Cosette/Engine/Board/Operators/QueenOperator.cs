using System;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class QueenOperator
    {
        public static int GetAvailableMoves(BoardState boardState, int color, Span<Move> moves, int offset)
        {
            var enemyColor = ColorOperations.Invert(color);
            var queens = boardState.Pieces[color][Piece.Queen];

            while (queens != 0)
            {
                var piece = BitOperations.GetLsb(queens);
                queens = BitOperations.PopLsb(queens);

                var from = BitOperations.BitScan(piece);
                var availableMoves = QueenMovesGenerator.GetMoves(boardState.OccupancySummary, from) & ~boardState.Occupancy[color];

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);
                    availableMoves = BitOperations.PopLsb(availableMoves);

                    var flags = (field & boardState.Occupancy[enemyColor]) != 0 ? MoveFlags.Capture : MoveFlags.Quiet;
                    moves[offset++] = new Move(from, fieldIndex, flags);
                }
            }

            return offset;
        }

        public static int GetAvailableQMoves(BoardState boardState, int color, Span<Move> moves, int offset)
        {
            var enemyColor = ColorOperations.Invert(color);
            var queens = boardState.Pieces[color][Piece.Queen];

            while (queens != 0)
            {
                var piece = BitOperations.GetLsb(queens);
                queens = BitOperations.PopLsb(queens);

                var from = BitOperations.BitScan(piece);
                var availableMoves = QueenMovesGenerator.GetMoves(boardState.OccupancySummary, from) & boardState.Occupancy[enemyColor];

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

        public static int GetMobility(BoardState boardState, int color)
        {
            var centerMobility = 0;
            var extendedCenterMobility = 0;
            var outsideMobility = 0;

            var queens = boardState.Pieces[color][Piece.Queen];

            while (queens != 0)
            {
                var piece = BitOperations.GetLsb(queens);
                queens = BitOperations.PopLsb(queens);

                var from = BitOperations.BitScan(piece);
                var availableMoves = QueenMovesGenerator.GetMoves(boardState.OccupancySummary, from);

                centerMobility += (int)BitOperations.Count(availableMoves & EvaluationConstants.Center);
                extendedCenterMobility += (int)BitOperations.Count(availableMoves & EvaluationConstants.ExtendedCenter);
                outsideMobility += (int)BitOperations.Count(availableMoves & EvaluationConstants.Outside);
            }

            return EvaluationConstants.CenterMobilityModifier * centerMobility +
                   EvaluationConstants.ExtendedCenterMobilityModifier * extendedCenterMobility +
                   EvaluationConstants.OutsideMobilityModifier * outsideMobility;
        }

        public static bool IsMoveLegal(BoardState boardState, Move move)
        {
            var enemyColor = ColorOperations.Invert(boardState.ColorToMove);
            var availableMoves = QueenMovesGenerator.GetMoves(boardState.OccupancySummary, move.From);
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