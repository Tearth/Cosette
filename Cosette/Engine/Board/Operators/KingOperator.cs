using System;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board.Operators
{
    public static class KingOperator
    {
        public static int GetAvailableMoves(BoardState boardState, Color color, Span<Move> moves, int offset)
        {
            var friendlyOccupancy = color == Color.White ? boardState.WhiteOccupancy : boardState.BlackOccupancy;
            var enemyOccupancy = color == Color.White ? boardState.BlackOccupancy : boardState.WhiteOccupancy;
            var kings = color == Color.White ? boardState.WhitePieces[(int)Piece.King] : boardState.BlackPieces[(int)Piece.King];
            Span<Piece> attackingPieces = stackalloc Piece[6];

            while (kings != 0)
            {
                var piece = BitOperations.GetLsb(kings);
                kings = BitOperations.PopLsb(kings);

                var from = BitOperations.BitScan(piece);
                var availableMoves = KingMovesGenerator.GetMoves(from) & ~friendlyOccupancy;

                while (availableMoves != 0)
                {
                    var field = BitOperations.GetLsb(availableMoves);
                    availableMoves = BitOperations.PopLsb(availableMoves);
                    var fieldIndex = BitOperations.BitScan(field);

                    moves[offset++] = new Move(from, fieldIndex, Piece.King, (field & enemyOccupancy) != 0 ? MoveFlags.Kill : MoveFlags.None);
                }

                if (color == Color.White)
                {
                    if ((boardState.Castling & Castling.WhiteShort) != 0 && (boardState.Occupancy & 6) == 0)
                    {
                        if (boardState.GetAttackingPiecesAtField(color, 1, attackingPieces) == 0 &&
                            boardState.GetAttackingPiecesAtField(color, 2, attackingPieces) == 0 &&
                            boardState.GetAttackingPiecesAtField(color, 3, attackingPieces) == 0)
                        {
                            moves[offset++] = new Move(3, 1, Piece.King, MoveFlags.Castling);
                        }
                    }
                    else if ((boardState.Castling & Castling.WhiteLong) != 0 && (boardState.Occupancy & 112) == 0)
                    {
                        if (boardState.GetAttackingPiecesAtField(color, 3, attackingPieces) == 0 &&
                            boardState.GetAttackingPiecesAtField(color, 4, attackingPieces) == 0 &&
                            boardState.GetAttackingPiecesAtField(color, 5, attackingPieces) == 0)
                        {
                            moves[offset++] = new Move(3, 5, Piece.King, MoveFlags.Castling);
                        }
                    }
                }
                else
                {
                    if ((boardState.Castling & Castling.BlackShort) != 0 && (boardState.Occupancy & 432345564227567616) == 0)
                    {
                        if (boardState.GetAttackingPiecesAtField(color, 57, attackingPieces) == 0 &&
                            boardState.GetAttackingPiecesAtField(color, 58, attackingPieces) == 0 &&
                            boardState.GetAttackingPiecesAtField(color, 59, attackingPieces) == 0)
                        {
                            moves[offset++] = new Move(59, 57, Piece.King, MoveFlags.Castling);
                        }
                    }
                    else if ((boardState.Castling & Castling.BlackLong) != 0 && (boardState.Occupancy & 8070450532247928832) == 0)
                    {
                        if (boardState.GetAttackingPiecesAtField(color, 59, attackingPieces) == 0 &&
                            boardState.GetAttackingPiecesAtField(color, 60, attackingPieces) == 0 &&
                            boardState.GetAttackingPiecesAtField(color, 61, attackingPieces) == 0)
                        {
                            moves[offset++] = new Move(59, 61, Piece.King, MoveFlags.Castling);
                        }
                    }
                }
            }

            return offset;
        }
    }
}