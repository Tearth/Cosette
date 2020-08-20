using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class PawnOperator
    {
        public static int GetAvailableMoves(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            offset = GetSinglePush(boardState, color, moves, offset);
            offset = GetDoublePush(boardState, color, moves, offset);
            offset = GetDiagonalAttacks(boardState, color, color == Color.White ? 9 : 7, BoardConstants.AFile, moves, offset);
            offset = GetDiagonalAttacks(boardState, color, color == Color.White ? 7 : 9, BoardConstants.HFile, moves, offset);

            return offset;
        }

        private static int GetSinglePush(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            int shift;
            ulong promotionRank, pawns;

            if (color == Color.White)
            {
                shift = 8;
                promotionRank = BoardConstants.HRank;
                pawns = boardState.WhitePieces[(int) Piece.Pawn];
                pawns = (pawns << 8) & ~boardState.Occupancy;
            }
            else
            {
                shift = -8;
                promotionRank = BoardConstants.ARank;
                pawns = boardState.BlackPieces[(int)Piece.Pawn];
                pawns = (pawns >> 8) & ~boardState.Occupancy;
            }

            while (pawns != 0)
            {
                var piece = BitOperations.GetLsb(pawns);
                pawns = BitOperations.PopLsb(pawns);

                var from = BitOperations.BitScan(piece) - shift;
                var to = BitOperations.BitScan(piece);

                if ((piece & promotionRank) != 0)
                {
                    moves[offset++] = new Move(from, to, Piece.Pawn, MoveFlags.Promotion | MoveFlags.KnightPromotion);
                    moves[offset++] = new Move(from, to, Piece.Pawn, MoveFlags.Promotion | MoveFlags.BishopPromotion);
                    moves[offset++] = new Move(from, to, Piece.Pawn, MoveFlags.Promotion | MoveFlags.RookPromotion);
                    moves[offset++] = new Move(from, to, Piece.Pawn, MoveFlags.Promotion | MoveFlags.QueenPromotion);
                }
                else
                {
                    moves[offset++] = new Move(from, to, Piece.Pawn, 0);
                }
            }

            return offset;
        }

        private static int GetDoublePush(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            int shift;
            ulong startRank, pawns;

            if (color == Color.White)
            {
                shift = 16;
                startRank = BoardConstants.BRank;
                pawns = boardState.WhitePieces[(int)Piece.Pawn];
                pawns = ((pawns & startRank) << 8) & ~boardState.Occupancy;
                pawns = (pawns << 8) & ~boardState.Occupancy;
            }
            else
            {
                shift = -16;
                startRank = BoardConstants.GRank;
                pawns = boardState.BlackPieces[(int)Piece.Pawn];
                pawns = ((pawns & startRank) >> 8) & ~boardState.Occupancy;
                pawns = (pawns >> 8) & ~boardState.Occupancy;
            }

            while (pawns != 0)
            {
                var piece = BitOperations.GetLsb(pawns);
                pawns = BitOperations.PopLsb(pawns);

                var from = BitOperations.BitScan(piece) - shift;
                var to = BitOperations.BitScan(piece);

                moves[offset++] = new Move(from, to, Piece.Pawn, 0);
            }

            return offset;
        }

        private static int GetDiagonalAttacks(BoardState boardState, Color color, int shift, ulong prohibitedFile, Span<Move> moves, int offset)
        {
            ulong promotionRank, enemyOccupancy, pawns;

            if (color == Color.White)
            {
                promotionRank = BoardConstants.HRank;
                enemyOccupancy = boardState.BlackOccupancy;
                pawns = boardState.WhitePieces[(int)Piece.Pawn];
                pawns = ((pawns & ~prohibitedFile) << shift) & enemyOccupancy;
            }
            else
            {
                promotionRank = BoardConstants.ARank;
                enemyOccupancy = boardState.WhiteOccupancy;
                pawns = boardState.BlackPieces[(int)Piece.Pawn];
                pawns = ((pawns & ~prohibitedFile) >> shift) & enemyOccupancy;
            }

            while (pawns != 0)
            {
                var piece = BitOperations.GetLsb(pawns);
                pawns = BitOperations.PopLsb(pawns);

                var from = BitOperations.BitScan(piece) - shift;
                var to = BitOperations.BitScan(piece);

                if ((piece & promotionRank) != 0)
                {
                    moves[offset++] = new Move(from, to, Piece.Pawn, MoveFlags.Kill | MoveFlags.Promotion | MoveFlags.KnightPromotion);
                    moves[offset++] = new Move(from, to, Piece.Pawn, MoveFlags.Kill | MoveFlags.Promotion | MoveFlags.BishopPromotion);
                    moves[offset++] = new Move(from, to, Piece.Pawn, MoveFlags.Kill | MoveFlags.Promotion | MoveFlags.RookPromotion);
                    moves[offset++] = new Move(from, to, Piece.Pawn, MoveFlags.Kill | MoveFlags.Promotion | MoveFlags.QueenPromotion);
                }
                else
                {
                    moves[offset++] = new Move(from, to, Piece.Pawn, MoveFlags.Kill);
                }
            }

            return offset;
        }
    }
}
