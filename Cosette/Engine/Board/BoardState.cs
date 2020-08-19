using System;
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
            Occupancy = 18446462598732906495;
        }

        public void GetAvailableMoves(Span<Move> moves)
        {
            moves[0].From = 123;
        }
    }
}
