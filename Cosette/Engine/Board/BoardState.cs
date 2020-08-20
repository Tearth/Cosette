using System;
using Cosette.Engine.Board.Operators;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board
{
    public class BoardState
    {
        public ulong[] WhitePieces { get; set; }
        public ulong[] BlackPieces { get; set; }
        public ulong WhiteOccupancy { get; set; }
        public ulong BlackOccupancy { get; set; }
        public ulong Occupancy { get; set; }

        public void SetDefaultState()
        {
            WhitePieces = new ulong[6];
            BlackPieces = new ulong[6];

            WhitePieces[(int) Piece.Pawn] = 65280;
            WhitePieces[(int) Piece.Rook] = 129;
            WhitePieces[(int) Piece.Knight] = 66;
            WhitePieces[(int) Piece.Bishop] = 36;
            WhitePieces[(int) Piece.Queen] = 16;
            WhitePieces[(int) Piece.King] = 8;

            BlackPieces[(int) Piece.Pawn] = 71776119061217280;
            BlackPieces[(int) Piece.Rook] = 9295429630892703744;
            BlackPieces[(int) Piece.Knight] = 4755801206503243776;
            BlackPieces[(int) Piece.Bishop] = 2594073385365405696;
            BlackPieces[(int) Piece.Queen] = 1152921504606846976;
            BlackPieces[(int) Piece.King] = 576460752303423488;

            WhiteOccupancy = 65535;
            BlackOccupancy = 18446462598732840960;
            Occupancy = WhiteOccupancy | BlackOccupancy;
        }

        public int GetAvailableMoves(Span<Move> moves, Color color)
        {
            var movesCount = PawnOperator.GetAvailableMoves(this, color, moves, 0);
            movesCount = RookOperator.GetAvailableMoves(this, color, moves, movesCount);
            movesCount = BishopOperator.GetAvailableMoves(this, color, moves, movesCount);
            movesCount = QueenOperator.GetAvailableMoves(this, color, moves, movesCount);
            movesCount = KnightOperator.GetAvailableMoves(this, color, moves, movesCount);
            movesCount = KingOperator.GetAvailableMoves(this, color, moves, movesCount);

            return movesCount;
        }

        public void MakeMove(Move move, Color color)
        {
            var pieces = color == Color.White ? WhitePieces : BlackPieces;
            var occupancy = color == Color.White ? WhiteOccupancy : BlackOccupancy;

            if (move.Flags == MoveFlags.None)
            {
                pieces[move.Piece] &= ~(1ul << move.From);
                pieces[move.Piece] |= 1ul << move.To;

                occupancy &= ~(1ul << move.From);
                occupancy |= 1ul << move.To;

            }
            else if ((move.Flags & MoveFlags.Kill) != 0)
            {

            }

            WhiteOccupancy = color == Color.White ? occupancy : WhiteOccupancy;
            BlackOccupancy = color == Color.Black ? occupancy : BlackOccupancy;
            Occupancy = WhiteOccupancy | BlackOccupancy;
        }

        public void UndoMove(Move move, Color color)
        {
            var pieces = color == Color.White ? WhitePieces : BlackPieces;
            var occupancy = color == Color.White ? WhiteOccupancy : BlackOccupancy;

            if (move.Flags == MoveFlags.None)
            {
                pieces[move.Piece] &= ~(1ul << move.To);
                pieces[move.Piece] |= 1ul << move.From;

                occupancy &= ~(1ul << move.To);
                occupancy |= 1ul << move.From;
            }

            WhiteOccupancy = color == Color.White ? occupancy : WhiteOccupancy;
            WhiteOccupancy = color == Color.Black ? occupancy : BlackOccupancy;
            Occupancy = WhiteOccupancy | BlackOccupancy;
        }
    }
}
