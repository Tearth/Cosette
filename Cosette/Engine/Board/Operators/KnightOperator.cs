using System;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class KnightOperator
    {
        public static int GetLoudMoves(BoardState boardState, Span<Move> moves, int offset, ulong evasionMask)
        {
            var color = boardState.ColorToMove;
            var enemyColor = ColorOperations.Invert(color);
            var knights = boardState.Pieces[color][Piece.Knight];

            while (knights != 0)
            {
                var piece = BitOperations.GetLsb(knights);
                knights = BitOperations.PopLsb(knights);

                var from = BitOperations.BitScan(piece);
                var availableMoves = KnightMovesGenerator.GetMoves(from) & boardState.Occupancy[enemyColor];
                availableMoves &= evasionMask;

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

        public static int GetQuietMoves(BoardState boardState, Span<Move> moves, int offset, ulong evasionMask)
        {
            var color = boardState.ColorToMove;
            var enemyColor = ColorOperations.Invert(color);
            var knights = boardState.Pieces[color][Piece.Knight];

            while (knights != 0)
            {
                var piece = BitOperations.GetLsb(knights);
                knights = BitOperations.PopLsb(knights);

                var from = BitOperations.BitScan(piece);
                var availableMoves = KnightMovesGenerator.GetMoves(from) & ~boardState.OccupancySummary;
                availableMoves &= evasionMask;

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
            var knights = boardState.Pieces[color][Piece.Knight];

            while (knights != 0)
            {
                var piece = BitOperations.GetLsb(knights);
                knights = BitOperations.PopLsb(knights);

                var from = BitOperations.BitScan(piece);
                var availableMoves = KnightMovesGenerator.GetMoves(from) & boardState.Occupancy[enemyColor];

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

            var knights = boardState.Pieces[color][Piece.Knight];

            while (knights != 0)
            {
                var piece = BitOperations.GetLsb(knights);
                knights = BitOperations.PopLsb(knights);

                var from = BitOperations.BitScan(piece);
                var availableMoves = KnightMovesGenerator.GetMoves(from);

                centerMobility += (int)BitOperations.Count(availableMoves & EvaluationConstants.ExtendedCenter);
                outsideMobility += (int)BitOperations.Count(availableMoves & EvaluationConstants.Outside);

                fieldsAttackedByColor |= availableMoves;
            }

            return (centerMobility, outsideMobility);
        }

        public static bool IsMoveLegal(BoardState boardState, Move move)
        {
            var enemyColor = ColorOperations.Invert(boardState.ColorToMove);
            var availableMoves = KnightMovesGenerator.GetMoves(move.From);
            var toField = 1ul << move.To;

            if (move.IsSinglePush() && (availableMoves & toField) != 0 && (boardState.OccupancySummary & toField) == 0)
            {
                return true;
            }

            if (move.IsCapture() && (availableMoves & toField) != 0 && (boardState.Occupancy[enemyColor] & toField) != 0)
            {
                return true;
            }

            return false;
        }
    }
}