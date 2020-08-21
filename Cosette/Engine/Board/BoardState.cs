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
        public ulong WhiteEnPassant { get; set; }
        public ulong BlackEnPassant { get; set; }

        private FastStack<Piece> _killedPieces;
        private FastStack<ulong> _enPassants;

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

            _killedPieces = new FastStack<Piece>(16);
            _enPassants = new FastStack<ulong>(16);
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
            var enPassant = color == Color.White ? WhiteEnPassant : BlackEnPassant;
            _enPassants.Push(enPassant);

            if (move.Flags == MoveFlags.None)
            {
                MovePiece(color, move.Piece, move.From, move.To);
            }
            else if ((move.Flags & MoveFlags.DoublePush) != 0)
            {
                MovePiece(color, move.Piece, move.From, move.To);
                enPassant |= color == Color.White ? 1ul << move.To - 8 : 1ul << move.To + 8;

                WhiteEnPassant = color == Color.White ? enPassant : WhiteEnPassant;
                BlackEnPassant = color == Color.Black ? enPassant : BlackEnPassant;
            }
            else if ((move.Flags & MoveFlags.Kill) != 0)
            {
                var enemyColor = ColorOperations.Invert(color);
                var killedPiece = GetPiece(enemyColor, move.To);

                RemovePiece(enemyColor, killedPiece, move.To);
                MovePiece(color, move.Piece, move.From, move.To);

                _killedPieces.Push(killedPiece);
            }
            else if ((move.Flags & MoveFlags.EnPassant) != 0)
            {
                var enemyColor = ColorOperations.Invert(color);
                var enemyPieceField = color == Color.White ? (byte)(move.To - 8) : (byte)(move.To + 8);
                var killedPiece = GetPiece(enemyColor, enemyPieceField);

                RemovePiece(enemyColor, killedPiece, move.To);
                MovePiece(color, move.Piece, move.From, move.To);

                _killedPieces.Push(killedPiece);
            }

            WhiteEnPassant = color == Color.White ? WhiteEnPassant : 0;
            BlackEnPassant = color == Color.Black ? BlackEnPassant : 0;
        }

        public void UndoMove(Move move, Color color)
        {
            if (move.Flags == MoveFlags.None)
            {
                MovePiece(color, move.Piece, move.To, move.From);
            }
            else if ((move.Flags & MoveFlags.DoublePush) != 0)
            {
                MovePiece(color, move.Piece, move.To, move.From);
            }
            else if ((move.Flags & MoveFlags.Kill) != 0)
            {
                var enemyColor = ColorOperations.Invert(color);
                var killedPiece = _killedPieces.Pop();

                MovePiece(color, move.Piece, move.To, move.From);
                AddPiece(enemyColor, killedPiece, move.To);
            }
            else if ((move.Flags & MoveFlags.EnPassant) != 0)
            {
                var enemyColor = ColorOperations.Invert(color);
                var enemyPieceField = color == Color.White ? (byte)(move.To - 8) : (byte)(move.To + 8);
                var killedPiece = _killedPieces.Pop();

                MovePiece(color, move.Piece, move.To, move.From);
                AddPiece(enemyColor, killedPiece, enemyPieceField);
            }

            var enPassant = _enPassants.Pop();
            WhiteEnPassant = color == Color.White ? enPassant : WhiteEnPassant;
            BlackEnPassant = color == Color.Black ? enPassant : BlackEnPassant;
        }

        public int GetAttackingPiecesAtField(Color color, byte field, Span<Piece> attackingPieces)
        {
            var attackingPiecesCount = 0;
            var enemyPieces = color == Color.White ? BlackPieces : WhitePieces;
            var enemyOccupancy = color == Color.White ? BlackOccupancy : WhiteOccupancy;

            var fileRankAttacks = RookMovesGenerator.GetMoves(Occupancy, field) & enemyOccupancy;
            var diagonalAttacks = BishopMovesGenerator.GetMoves(Occupancy, field) & enemyOccupancy;

            var attackingRooks = fileRankAttacks & enemyPieces[(int) Piece.Rook];
            var attackingBishops = diagonalAttacks & enemyPieces[(int) Piece.Bishop];
            var attackingQueens = (fileRankAttacks | diagonalAttacks) & enemyPieces[(int) Piece.Queen];
            var attackingKnights = KnightMovesGenerator.GetMoves(field) & enemyPieces[(int) Piece.Knight];
            var attackingKings = KingMovesGenerator.GetMoves(field) & enemyPieces[(int) Piece.King];
            //var attackingPawns = 

            if (attackingRooks != 0)
            {
                attackingPieces[attackingPiecesCount++] = Piece.Rook;
            }
            if (attackingBishops != 0)
            {
                attackingPieces[attackingPiecesCount++] = Piece.Bishop;
            }
            if (attackingQueens != 0)
            {
                attackingPieces[attackingPiecesCount++] = Piece.Queen;
            }
            if (attackingKnights != 0)
            {
                attackingPieces[attackingPiecesCount++] = Piece.Knight;
            }
            if (attackingKings != 0)
            {
                attackingPieces[attackingPiecesCount++] = Piece.King;
            }

            return attackingPiecesCount;
        }

        public bool IsKingChecked(Color color)
        {
            Span<Piece> attackingPieces = stackalloc Piece[6];
            var king = color == Color.White ? WhitePieces[(int)Piece.King] : BlackPieces[(int)Piece.King];

            if (king != 0)
            {
                var kingField = BitOperations.BitScan(king);
                var attackingPiecesCount = GetAttackingPiecesAtField(color, (byte)kingField, attackingPieces);

                if (attackingPiecesCount != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void MovePiece(Color color, Piece piece, byte from, byte to)
        {
            var pieces = color == Color.White ? WhitePieces : BlackPieces;
            var occupancy = color == Color.White ? WhiteOccupancy : BlackOccupancy;

            pieces[(int) piece] &= ~(1ul << from);
            pieces[(int) piece] |= 1ul << to;

            occupancy &= ~(1ul << from);
            occupancy |= 1ul << to;

            WhiteOccupancy = color == Color.White ? occupancy : WhiteOccupancy;
            BlackOccupancy = color == Color.Black ? occupancy : BlackOccupancy;
            Occupancy = WhiteOccupancy | BlackOccupancy;
        }

        private Piece GetPiece(Color color, byte from)
        {
            var pieces = color == Color.White ? WhitePieces : BlackPieces;
            var field = 1ul << from;

            for (var i = 0; i < pieces.Length; i++)
            {
                if ((pieces[i] & field) != 0)
                {
                    return (Piece) i;
                }
            }

            throw new InvalidOperationException();
        }

        private void AddPiece(Color color, Piece piece, byte field)
        {
            var pieces = color == Color.White ? WhitePieces : BlackPieces;
            var occupancy = color == Color.White ? WhiteOccupancy : BlackOccupancy;

            pieces[(byte) piece] |= 1ul << field;
            occupancy |= 1ul << field;

            WhiteOccupancy = color == Color.White ? occupancy : WhiteOccupancy;
            BlackOccupancy = color == Color.Black ? occupancy : BlackOccupancy;
            Occupancy = WhiteOccupancy | BlackOccupancy;
        }

        private void RemovePiece(Color color, Piece piece, byte from)
        {
            var pieces = color == Color.White ? WhitePieces : BlackPieces;
            var occupancy = color == Color.White ? WhiteOccupancy : BlackOccupancy;

            pieces[(byte) piece] &= ~(1ul << from);
            occupancy &= ~(1ul << from);

            WhiteOccupancy = color == Color.White ? occupancy : WhiteOccupancy;
            BlackOccupancy = color == Color.Black ? occupancy : BlackOccupancy;
            Occupancy = WhiteOccupancy | BlackOccupancy;
        }
    }
}
