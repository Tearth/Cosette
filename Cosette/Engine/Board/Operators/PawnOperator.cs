using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Board.Operators
{
    public static class PawnOperator
    {
        public static int GetLoudMoves(BoardState boardState, Span<Move> moves, int offset)
        {
            var color = boardState.ColorToMove;

            offset = GetSinglePush(boardState, moves, offset, true);
            offset = GetDiagonalAttacks(boardState, color == Color.White ? 9 : 7, BoardConstants.AFile, moves, offset);
            offset = GetDiagonalAttacks(boardState, color == Color.White ? 7 : 9, BoardConstants.HFile, moves, offset);

            return offset;
        }

        public static int GetQuietMoves(BoardState boardState, Span<Move> moves, int offset)
        {
            offset = GetSinglePush(boardState, moves, offset, false);
            offset = GetDoublePush(boardState, moves, offset);

            return offset;
        }

        public static int GetAvailableCaptureMoves(BoardState boardState, Span<Move> moves, int offset)
        {
            var color = boardState.ColorToMove;

            offset = GetDiagonalAttacks(boardState, color == Color.White ? 9 : 7, BoardConstants.AFile, moves, offset);
            offset = GetDiagonalAttacks(boardState, color == Color.White ? 7 : 9, BoardConstants.HFile, moves, offset);

            return offset;
        }

        public static bool IsMoveLegal(BoardState boardState, Move move)
        {
            var enemyColor = ColorOperations.Invert(boardState.ColorToMove);
            var toField = 1ul << move.To;

            if (!move.IsCapture())
            {
                if (move.Flags == MoveFlags.Quiet || ((int)move.Flags & MoveFlagFields.Promotion) != 0)
                {
                    if ((boardState.OccupancySummary & toField) == 0)
                    {
                        return true;
                    }
                }
                else if (move.Flags == MoveFlags.DoublePush)
                {
                    var middleField = 1ul << ((move.From + move.To) / 2);
                    if ((boardState.OccupancySummary & middleField) == 0 && (boardState.OccupancySummary & toField) == 0)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (move.Flags == MoveFlags.EnPassant)
                {
                    if ((boardState.EnPassant & toField) != 0)
                    {
                        return true;
                    }
                }
                else
                {
                    var difference = move.To - move.From;
                    var colorDifference = -(boardState.ColorToMove * 2 - 1) * difference;

                    if ((boardState.Occupancy[enemyColor] & toField) != 0 && (colorDifference == 7 || colorDifference == 9))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static int GetSinglePush(BoardState boardState, Span<Move> moves, int offset, bool promotionsMode)
        {
            int shift;
            ulong promotionRank, pawns;
            var color = boardState.ColorToMove;

            if (color == Color.White)
            {
                shift = 8;
                promotionRank = BoardConstants.HRank;
                pawns = boardState.Pieces[Color.White][Piece.Pawn];

                if (promotionsMode)
                {
                    pawns &= BoardConstants.NearPromotionAreaWhite;
                }
                else
                {
                    pawns &= ~BoardConstants.NearPromotionAreaWhite;
                }

                pawns = (pawns << 8) & ~boardState.OccupancySummary;
            }
            else
            {
                shift = -8;
                promotionRank = BoardConstants.ARank;
                pawns = boardState.Pieces[Color.Black][Piece.Pawn];

                if (promotionsMode)
                {
                    pawns &= BoardConstants.NearPromotionAreaBlack;
                }
                else
                {
                    pawns &= ~BoardConstants.NearPromotionAreaBlack;
                }

                pawns = (pawns >> 8) & ~boardState.OccupancySummary;
            }

            while (pawns != 0)
            {
                var piece = BitOperations.GetLsb(pawns);
                pawns = BitOperations.PopLsb(pawns);

                var from = BitOperations.BitScan(piece) - shift;
                var to = BitOperations.BitScan(piece);

                if (promotionsMode && (piece & promotionRank) != 0)
                {
                    moves[offset++] = new Move(from, to, MoveFlags.QueenPromotion);
                    moves[offset++] = new Move(from, to, MoveFlags.RookPromotion);
                    moves[offset++] = new Move(from, to, MoveFlags.KnightPromotion);
                    moves[offset++] = new Move(from, to, MoveFlags.BishopPromotion);
                    
                }
                else
                {
                    moves[offset++] = new Move(from, to, 0);
                }
            }

            return offset;
        }

        private static int GetDoublePush(BoardState boardState, Span<Move> moves, int offset)
        {
            int shift;
            ulong startRank, pawns;
            var color = boardState.ColorToMove;

            if (color == Color.White)
            {
                shift = 16;
                startRank = BoardConstants.BRank;
                pawns = boardState.Pieces[Color.White][Piece.Pawn];
                pawns = ((pawns & startRank) << 8) & ~boardState.OccupancySummary;
                pawns = (pawns << 8) & ~boardState.OccupancySummary;
            }
            else
            {
                shift = -16;
                startRank = BoardConstants.GRank;
                pawns = boardState.Pieces[Color.Black][Piece.Pawn];
                pawns = ((pawns & startRank) >> 8) & ~boardState.OccupancySummary;
                pawns = (pawns >> 8) & ~boardState.OccupancySummary;
            }

            while (pawns != 0)
            {
                var piece = BitOperations.GetLsb(pawns);
                pawns = BitOperations.PopLsb(pawns);

                var from = BitOperations.BitScan(piece) - shift;
                var to = BitOperations.BitScan(piece);

                moves[offset++] = new Move(from, to, MoveFlags.DoublePush);
            }

            return offset;
        }

        private static int GetDiagonalAttacks(BoardState boardState, int dir, ulong prohibitedFile, Span<Move> moves, int offset)
        {
            int shift;
            ulong promotionRank, enemyOccupancy, pawns;
            var color = boardState.ColorToMove;

            if (color == Color.White)
            {
                shift = dir;
                promotionRank = BoardConstants.HRank;
                enemyOccupancy = boardState.Occupancy[Color.Black] | boardState.EnPassant;
                pawns = boardState.Pieces[Color.White][Piece.Pawn];
                pawns = ((pawns & ~prohibitedFile) << dir) & enemyOccupancy;
            }
            else
            {
                shift = -dir;
                promotionRank = BoardConstants.ARank;
                enemyOccupancy = boardState.Occupancy[Color.White] | boardState.EnPassant;
                pawns = boardState.Pieces[Color.Black][Piece.Pawn];
                pawns = ((pawns & ~prohibitedFile) >> dir) & enemyOccupancy;
            }

            while (pawns != 0)
            {
                var piece = BitOperations.GetLsb(pawns);
                pawns = BitOperations.PopLsb(pawns);

                var from = BitOperations.BitScan(piece) - shift;
                var to = BitOperations.BitScan(piece);

                if ((piece & promotionRank) != 0)
                {
                    moves[offset++] = new Move(from, to, MoveFlags.QueenPromotionCapture);
                    moves[offset++] = new Move(from, to, MoveFlags.RookPromotionCapture);
                    moves[offset++] = new Move(from, to, MoveFlags.KnightPromotionCapture);
                    moves[offset++] = new Move(from, to, MoveFlags.BishopPromotionCapture);
                }
                else
                {
                    if ((piece & boardState.EnPassant) != 0)
                    {
                        moves[offset++] = new Move(from, to, MoveFlags.EnPassant);
                    }
                    else
                    {
                        moves[offset++] = new Move(from, to, MoveFlags.Capture);
                    }
                }
            }

            return offset;
        }
    }
}
