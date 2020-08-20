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
            offset = GetDiagonalAttacks(boardState, color, color == Color.White ? 9 : -7, BoardConstants.AFile, moves, offset);
            offset = GetDiagonalAttacks(boardState, color, color == Color.White ? 7 : -9, BoardConstants.HFile, moves, offset);

            return offset;
        }

        private static int GetSinglePush(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            var pushShift = color == Color.White ? 8 : -8;
            var promotionRank = color == Color.White ? BoardConstants.HRank : BoardConstants.ARank;
            var pawns = color == Color.White ? boardState.WhitePieces[(int)Piece.Pawn] : boardState.BlackPieces[(int)Piece.Pawn];
            var singlePush = (pawns << pushShift) & ~boardState.Occupancy;

            while (singlePush != 0)
            {
                var piece = BitOperations.GetLsb(singlePush);
                singlePush = BitOperations.PopLsb(singlePush);

                var from = BitOperations.BitScan(piece >> pushShift);
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
            var pushShift = color == Color.White ? 8 : -8;
            var launchRank = color == Color.White ? BoardConstants.BRank : BoardConstants.GRank;
            var pawns = color == Color.White ? boardState.WhitePieces[(int)Piece.Pawn] : boardState.BlackPieces[(int)Piece.Pawn];

            var pawnsAtLaunchRank = pawns & launchRank;
            var singlePush = (pawnsAtLaunchRank << pushShift) & ~boardState.Occupancy;
            var doublePush = (singlePush << pushShift) & ~boardState.Occupancy;

            while (doublePush != 0)
            {
                var piece = BitOperations.GetLsb(doublePush);
                doublePush = BitOperations.PopLsb(doublePush);

                var from = BitOperations.BitScan(piece >> (2 * pushShift));
                var to = BitOperations.BitScan(piece);

                moves[offset++] = new Move(from, to, Piece.Pawn, 0);
            }

            return offset;
        }

        private static int GetDiagonalAttacks(BoardState boardState, Color color, int shift, ulong prohibitedFile, Span<Move> moves, int offset)
        {
            var enemyOccupancy = color == Color.White ? boardState.BlackOccupancy : boardState.WhiteOccupancy;
            var promotionRank = color == Color.White ? BoardConstants.HRank : BoardConstants.ARank;
            var pawns = color == Color.White ? boardState.WhitePieces[(int)Piece.Pawn] : boardState.BlackPieces[(int)Piece.Pawn];

            var attacks = ((pawns & ~prohibitedFile) << shift) & enemyOccupancy;
            while (attacks != 0)
            {
                var piece = BitOperations.GetLsb(attacks);
                attacks = BitOperations.PopLsb(attacks);

                var from = BitOperations.BitScan(piece >> shift);
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
                    moves[offset++] = new Move(from, to, Piece.Pawn, MoveFlags.Kill);
                }
            }

            return offset;
        }
    }
}
