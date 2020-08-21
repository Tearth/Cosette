using System;
using Cosette.Engine.Board.Operators;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board
{
    public class BoardState
    {
        public ulong[][] Pieces { get; set; }
        public ulong[] Occupancy { get; set; }
        public ulong OccupancySummary { get; set; }
        public ulong[] EnPassant { get; set; }

        public Castling Castling { get; set; }

        private FastStack<Piece> _killedPieces;
        private FastStack<ulong> _enPassants;
        private FastStack<Castling> _castlings;
        private FastStack<Piece> _promotedPieces;

        public void SetDefaultState()
        {
            Pieces = new ulong[2][];
            Pieces[(int)Color.White] = new ulong[6];
            Pieces[(int)Color.Black] = new ulong[6];

            Occupancy = new ulong[2];
            EnPassant = new ulong[2];

            Pieces[(int)Color.White][(int) Piece.Pawn] = 65280;
            Pieces[(int)Color.White][(int) Piece.Rook] = 129;
            Pieces[(int)Color.White][(int) Piece.Knight] = 66;
            Pieces[(int)Color.White][(int) Piece.Bishop] = 36;
            Pieces[(int)Color.White][(int) Piece.Queen] = 16;
            Pieces[(int)Color.White][(int) Piece.King] = 8;

            Pieces[(int)Color.Black][(int) Piece.Pawn] = 71776119061217280;
            Pieces[(int)Color.Black][(int) Piece.Rook] = 9295429630892703744;
            Pieces[(int)Color.Black][(int) Piece.Knight] = 4755801206503243776;
            Pieces[(int)Color.Black][(int) Piece.Bishop] = 2594073385365405696;
            Pieces[(int)Color.Black][(int) Piece.Queen] = 1152921504606846976;
            Pieces[(int)Color.Black][(int) Piece.King] = 576460752303423488;

            Occupancy[(int)Color.White] = 65535;
            Occupancy[(int)Color.Black] = 18446462598732840960;
            OccupancySummary = Occupancy[(int)Color.White] | Occupancy[(int)Color.Black];

            Castling = Castling.Everything;

            _killedPieces = new FastStack<Piece>(16);
            _enPassants = new FastStack<ulong>(16);
            _castlings = new FastStack<Castling>(128);
            _promotedPieces = new FastStack<Piece>(16);
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
            _enPassants.Push(EnPassant[(int)color]);
            _castlings.Push(Castling);

            if (move.Flags == MoveFlags.None)
            {
                MovePiece(color, move.Piece, move.From, move.To);
            }
            else if ((move.Flags & MoveFlags.DoublePush) != 0)
            {
                MovePiece(color, move.Piece, move.From, move.To);
                EnPassant[(int)color] |= color == Color.White ? 1ul << move.To - 8 : 1ul << move.To + 8;
            }
            else if ((move.Flags & MoveFlags.Kill) != 0)
            {
                var enemyColor = ColorOperations.Invert(color);
                var killedPiece = GetPiece(enemyColor, move.To);

                RemovePiece(enemyColor, killedPiece, move.To);

                if (killedPiece == Piece.Rook)
                {
                    switch (move.To)
                    {
                        case 0:
                        {
                            Castling &= ~Castling.WhiteShort;
                            break;
                        }
                        case 7:
                        {
                            Castling &= ~Castling.WhiteLong;
                            break;
                        }
                        case 56:
                        {
                            Castling &= ~Castling.BlackShort;
                            break;
                        }
                        case 63:
                        {
                            Castling &= ~Castling.BlackLong;
                            break;
                        }
                    }
                }

                // Promotion
                if ((byte)move.Flags >= 16)
                {
                    var promotionPiece = GetPromotionPiece(move.Flags);
                    RemovePiece(color, move.Piece, move.From);
                    AddPiece(color, promotionPiece, move.To);
                    _promotedPieces.Push(promotionPiece);
                }
                else
                {
                    MovePiece(color, move.Piece, move.From, move.To);
                }

                _killedPieces.Push(killedPiece);
            }
            else if ((move.Flags & MoveFlags.Castling) != 0)
            {
                // Short castling
                if (move.From > move.To)
                {
                    if (color == Color.White)
                    {
                        MovePiece(Color.White, Piece.King, 3, 1);
                        MovePiece(Color.White, Piece.Rook, 0, 2);
                    }
                    else
                    {
                        MovePiece(Color.Black, Piece.King, 59, 57);
                        MovePiece(Color.Black, Piece.Rook, 56, 58);
                    }
                }
                // Long castling
                else
                {
                    if (color == Color.White)
                    {
                        MovePiece(Color.White, Piece.King, 3, 5);
                        MovePiece(Color.White, Piece.Rook, 7, 4);
                    }
                    else
                    {
                        MovePiece(Color.Black, Piece.King, 59, 61);
                        MovePiece(Color.Black, Piece.Rook, 63, 60);
                    }
                }

                if (color == Color.White)
                {
                    Castling &= ~Castling.WhiteShort;
                    Castling &= ~Castling.WhiteLong;
                }
                else
                {
                    Castling &= ~Castling.BlackShort;
                    Castling &= ~Castling.BlackLong;
                }
            }
            else if ((move.Flags & MoveFlags.EnPassant) != 0)
            {
                var enemyColor = ColorOperations.Invert(color);
                var enemyPieceField = color == Color.White ? (byte)(move.To - 8) : (byte)(move.To + 8);
                var killedPiece = GetPiece(enemyColor, enemyPieceField);

                RemovePiece(enemyColor, killedPiece, enemyPieceField);
                MovePiece(color, move.Piece, move.From, move.To);

                _killedPieces.Push(killedPiece);
            }
            else if ((byte)move.Flags  >= 16)
            {
                var promotionPiece = GetPromotionPiece(move.Flags);
                RemovePiece(color, move.Piece, move.From);
                AddPiece(color, promotionPiece, move.To);
                _promotedPieces.Push(promotionPiece);
            }

            if (move.Piece == Piece.King && move.Flags != MoveFlags.Castling)
            {
                if (color == Color.White)
                {
                    Castling &= ~Castling.WhiteShort;
                    Castling &= ~Castling.WhiteLong;
                }
                else
                {
                    Castling &= ~Castling.BlackShort;
                    Castling &= ~Castling.BlackLong;
                }
            }

            if (move.Piece == Piece.Rook)
            {
                if (color == Color.White)
                {
                    if (move.From == 0)
                    {
                        Castling &= ~Castling.WhiteShort;
                    }
                    else if (move.From == 7)
                    {
                        Castling &= ~Castling.WhiteLong;
                    }
                }
                else if (color == Color.Black)
                {
                    if (move.From == 56)
                    {
                        Castling &= ~Castling.BlackShort;
                    }
                    else if (move.From == 63)
                    {
                        Castling &= ~Castling.BlackLong;
                    }
                }
            }

            EnPassant[(int)ColorOperations.Invert(color)] = 0;
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

                // Promotion
                if ((byte)move.Flags >= 16)
                {
                    var promotionPiece = _promotedPieces.Pop();
                    RemovePiece(color, promotionPiece, move.To);
                    AddPiece(color, move.Piece, move.From);
                }
                else
                {
                    MovePiece(color, move.Piece, move.To, move.From);
                }

                AddPiece(enemyColor, killedPiece, move.To);
            }
            else if ((move.Flags & MoveFlags.Castling) != 0)
            {
                // Short castling
                if (move.From > move.To)
                {
                    if (color == Color.White)
                    {
                        MovePiece(Color.White, Piece.King, 1, 3);
                        MovePiece(Color.White, Piece.Rook, 2, 0);
                    }
                    else
                    {
                        MovePiece(Color.Black, Piece.King, 57, 59);
                        MovePiece(Color.Black, Piece.Rook, 58, 56);
                    }
                }
                // Long castling
                else
                {
                    if (color == Color.White)
                    {
                        MovePiece(Color.White, Piece.King, 5, 3);
                        MovePiece(Color.White, Piece.Rook, 4, 7);
                    }
                    else
                    {
                        MovePiece(Color.Black, Piece.King, 61, 59);
                        MovePiece(Color.Black, Piece.Rook, 60, 63);
                    }
                }
            }
            else if ((move.Flags & MoveFlags.EnPassant) != 0)
            {
                var enemyColor = ColorOperations.Invert(color);
                var enemyPieceField = color == Color.White ? (byte)(move.To - 8) : (byte)(move.To + 8);
                var killedPiece = _killedPieces.Pop();

                MovePiece(color, move.Piece, move.To, move.From);
                AddPiece(enemyColor, killedPiece, enemyPieceField);
            }
            else if ((byte)move.Flags >= 16)
            {
                var promotionPiece = _promotedPieces.Pop();
                RemovePiece(color, promotionPiece, move.To);
                AddPiece(color, move.Piece, move.From);
            }

            Castling = _castlings.Pop();
            EnPassant[(int)color] = _enPassants.Pop();
        }

        public int GetAttackingPiecesAtField(Color color, byte fieldIndex, Span<Piece> attackingPieces)
        {
            var attackingPiecesCount = 0;
            var enemyColor = ColorOperations.Invert(color);
            var enemyOccupancy = Occupancy[(int)ColorOperations.Invert(color)];

            var fileRankAttacks = RookMovesGenerator.GetMoves(OccupancySummary, fieldIndex) & enemyOccupancy;
            var diagonalAttacks = BishopMovesGenerator.GetMoves(OccupancySummary, fieldIndex) & enemyOccupancy;
            var jumpAttacks = KnightMovesGenerator.GetMoves(fieldIndex);
            var boxAttacks = KnightMovesGenerator.GetMoves(fieldIndex);

            var attackingRooks = fileRankAttacks & Pieces[(int)enemyColor][(int) Piece.Rook];
            var attackingBishops = diagonalAttacks & Pieces[(int)enemyColor][(int) Piece.Bishop];
            var attackingQueens = (fileRankAttacks | diagonalAttacks) & Pieces[(int)enemyColor][(int) Piece.Queen];
            var attackingKnights = jumpAttacks & Pieces[(int)enemyColor][(int) Piece.Knight];
            var attackingKings = boxAttacks & Pieces[(int)enemyColor][(int) Piece.King];

            var field = 1ul << fieldIndex;
            var potentialPawns = boxAttacks & Pieces[(int)enemyColor][(int)Piece.Pawn];
            var attackingPawns = color == Color.White ?
                field & ((potentialPawns >> 7) | (potentialPawns >> 9)) :
                field & ((potentialPawns << 7) | (potentialPawns << 9));

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

            if (attackingPawns != 0)
            {
                attackingPieces[attackingPiecesCount++] = Piece.Pawn;
            }

            return attackingPiecesCount;
        }

        public bool IsKingChecked(Color color)
        {
            Span<Piece> attackingPieces = stackalloc Piece[6];
            var king = Pieces[(int) color][(int) Piece.King];

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
            var occupancy = Occupancy[(int)color];

            Pieces[(int)color][(int) piece] &= ~(1ul << from);
            Pieces[(int)color][(int) piece] |= 1ul << to;

            occupancy &= ~(1ul << from);
            occupancy |= 1ul << to;

            Occupancy[(int) color] = occupancy;
            OccupancySummary = Occupancy[(int)Color.White] | Occupancy[(int)Color.Black];
        }

        private Piece GetPiece(Color color, byte from)
        {
            var field = 1ul << from;

            for (var i = 0; i < 6; i++)
            {
                if ((Pieces[(int)color][i] & field) != 0)
                {
                    return (Piece) i;
                }
            }

            throw new InvalidOperationException();
        }

        private void AddPiece(Color color, Piece piece, byte field)
        {
            var occupancy = Occupancy[(int)color];

            Pieces[(int)color][(int)piece] |= 1ul << field;
            occupancy |= 1ul << field;

            Occupancy[(int)color] = occupancy;
            OccupancySummary = Occupancy[(int)Color.White] | Occupancy[(int)Color.Black];
        }

        private void RemovePiece(Color color, Piece piece, byte from)
        {
            var occupancy = Occupancy[(int)color];

            Pieces[(int)color][(int)piece] &= ~(1ul << from);
            occupancy &= ~(1ul << from);

            Occupancy[(int)color] = occupancy;
            OccupancySummary = Occupancy[(int)Color.White] | Occupancy[(int)Color.Black];
        }

        private Piece GetPromotionPiece(MoveFlags flags)
        {
            if ((flags & MoveFlags.KnightPromotion) != 0)
            {
                return Piece.Knight;
            }
            
            if ((flags & MoveFlags.BishopPromotion) != 0)
            {
                return Piece.Bishop;
            }
            
            if ((flags & MoveFlags.RookPromotion) != 0)
            {
                return Piece.Rook;
            }
            
            if ((flags & MoveFlags.QueenPromotion) != 0)
            {
                return Piece.Queen;
            }

            throw new InvalidOperationException();
        }
    }
}
