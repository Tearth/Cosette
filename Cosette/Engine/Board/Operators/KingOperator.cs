using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class KingOperator
    {
        public static int GetLoudMoves(BoardState boardState, Span<Move> moves, int offset)
        {
            var color = boardState.ColorToMove;
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

            if (color == Color.White)
            {
                if (IsWhiteKingCastlingAvailable(boardState, color))
                {
                    moves[offset++] = new Move(3, 1, MoveFlags.KingCastle);
                }

                if (IsWhiteQueenCastlingAvailable(boardState, color))
                {
                    moves[offset++] = new Move(3, 5, MoveFlags.QueenCastle);
                }
            }
            else
            {
                if (IsBlackKingCastlingAvailable(boardState, color))
                {
                    moves[offset++] = new Move(59, 57, MoveFlags.KingCastle);
                }

                if (IsBlackQueenCastlingAvailable(boardState, color))
                {
                    moves[offset++] = new Move(59, 61, MoveFlags.QueenCastle);
                }
            }

            return offset;
        }

        public static int GetQuietMoves(BoardState boardState, Span<Move> moves, int offset)
        {
            var color = boardState.ColorToMove;
            var enemyColor = ColorOperations.Invert(color);
            var piece = boardState.Pieces[color][Piece.King];

            if (piece == 0)
            {
                return offset;
            }

            var from = BitOperations.BitScan(piece);
            var availableMoves = KingMovesGenerator.GetMoves(from) & ~boardState.OccupancySummary;

            while (availableMoves != 0)
            {
                var field = BitOperations.GetLsb(availableMoves);
                var fieldIndex = BitOperations.BitScan(field);
                availableMoves = BitOperations.PopLsb(availableMoves);

                moves[offset++] = new Move(from, fieldIndex, MoveFlags.Quiet);
            }

            return offset;
        }

        public static int GetAvailableCaptureMoves(BoardState boardState, Span<Move> moves, int offset)
        {
            var color = boardState.ColorToMove;
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

        public static bool IsMoveLegal(BoardState boardState, Move move)
        {
            var enemyColor = ColorOperations.Invert(boardState.ColorToMove);
            var availableMoves = KingMovesGenerator.GetMoves(move.From);
            var toField = 1ul << move.To;

            if (move.Flags == MoveFlags.Quiet && (availableMoves & toField) != 0 && (boardState.OccupancySummary & toField) == 0)
            {
                return true;
            }

            if (move.Flags == MoveFlags.Capture && (availableMoves & toField) != 0 && (boardState.Occupancy[enemyColor] & toField) != 0)
            {
                return true;
            }

            if (move.Flags == MoveFlags.KingCastle)
            {
                if (boardState.ColorToMove == Color.White && IsWhiteKingCastlingAvailable(boardState, boardState.ColorToMove))
                {
                    return true;
                }
                else if (boardState.ColorToMove == Color.Black && IsBlackKingCastlingAvailable(boardState, boardState.ColorToMove))
                {
                    return true;
                }
            }

            if (move.Flags == MoveFlags.QueenCastle)
            {
                if (boardState.ColorToMove == Color.White && IsWhiteQueenCastlingAvailable(boardState, boardState.ColorToMove))
                {
                    return true;
                }
                else if (boardState.ColorToMove == Color.Black && IsBlackQueenCastlingAvailable(boardState, boardState.ColorToMove))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsWhiteKingCastlingAvailable(BoardState boardState, int color)
        {
            if ((boardState.Castling & Castling.WhiteShort) != 0 && (boardState.OccupancySummary & 6) == 0)
            {
                if (!boardState.IsFieldAttacked(color, 1) && !boardState.IsFieldAttacked(color, 2) && !boardState.IsFieldAttacked(color, 3))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsWhiteQueenCastlingAvailable(BoardState boardState, int color)
        {
            if ((boardState.Castling & Castling.WhiteLong) != 0 && (boardState.OccupancySummary & 112) == 0)
            {
                if (!boardState.IsFieldAttacked(color, 3) && !boardState.IsFieldAttacked(color, 4) && !boardState.IsFieldAttacked(color, 5))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsBlackKingCastlingAvailable(BoardState boardState, int color)
        {
            if ((boardState.Castling & Castling.BlackShort) != 0 && (boardState.OccupancySummary & 432345564227567616) == 0)
            {
                if (!boardState.IsFieldAttacked(color, 57) && !boardState.IsFieldAttacked(color, 58) && !boardState.IsFieldAttacked(color, 59))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsBlackQueenCastlingAvailable(BoardState boardState, int color)
        {
            if ((boardState.Castling & Castling.BlackLong) != 0 && (boardState.OccupancySummary & 8070450532247928832) == 0)
            {
                if (!boardState.IsFieldAttacked(color, 59) && !boardState.IsFieldAttacked(color, 60) && !boardState.IsFieldAttacked(color, 61))
                {
                    return true;
                }
            }

            return false;
        }
    }
}