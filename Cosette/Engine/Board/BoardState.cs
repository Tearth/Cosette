using System;
using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Board.Operators;
using Cosette.Engine.Common;
using Cosette.Engine.Fen;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Board
{
    public class BoardState
    {
        public ulong[][] Pieces { get; set; }
        public ulong[] Occupancy { get; set; }
        public ulong OccupancySummary { get; set; }
        public ulong EnPassant { get; set; }
        public Castling Castling { get; set; }
        public int ColorToMove { get; set; }
        public int MovesCount { get; set; }
        public int IrreversibleMovesCount { get; set; }
        public int NullMoves { get; set; }

        public bool[] CastlingDone { get; set; }
        public int[] Material { get; set; }

        public int[] PieceTable { get; set; }
        public int MaterialAtOpening;

        public ulong Hash { get; set; }
        public ulong PawnHash { get; set; }

        private readonly FastStack<int> _killedPieces;
        private readonly FastStack<ulong> _enPassants;
        private readonly FastStack<Castling> _castlings;
        private readonly FastStack<int> _promotedPieces;
        private readonly FastStack<ulong> _hashes;
        private readonly FastStack<ulong> _pawnHashes;
        private readonly FastStack<int> _irreversibleMovesCounts;


        public BoardState()
        {
            Pieces = new ulong[2][];
            Pieces[Color.White] = new ulong[6];
            Pieces[Color.Black] = new ulong[6];

            Occupancy = new ulong[2];
            CastlingDone = new bool[2];
            Material = new int[2];

            PieceTable = new int[64];

            _killedPieces = new FastStack<int>(512);
            _enPassants = new FastStack<ulong>(512);
            _castlings = new FastStack<Castling>(512);
            _promotedPieces = new FastStack<int>(512);
            _hashes = new FastStack<ulong>(512);
            _pawnHashes = new FastStack<ulong>(512);
            _irreversibleMovesCounts = new FastStack<int>(512);
        }

        public void SetDefaultState()
        {
            Pieces[Color.White][Piece.Pawn] = 65280;
            Pieces[Color.White][Piece.Rook] = 129;
            Pieces[Color.White][Piece.Knight] = 66;
            Pieces[Color.White][Piece.Bishop] = 36;
            Pieces[Color.White][Piece.Queen] = 16;
            Pieces[Color.White][Piece.King] = 8;

            Pieces[Color.Black][Piece.Pawn] = 71776119061217280;
            Pieces[Color.Black][Piece.Rook] = 9295429630892703744;
            Pieces[Color.Black][Piece.Knight] = 4755801206503243776;
            Pieces[Color.Black][Piece.Bishop] = 2594073385365405696;
            Pieces[Color.Black][Piece.Queen] = 1152921504606846976;
            Pieces[Color.Black][Piece.King] = 576460752303423488;

            Occupancy[Color.White] = 65535;
            Occupancy[Color.Black] = 18446462598732840960;
            OccupancySummary = Occupancy[Color.White] | Occupancy[Color.Black];

            EnPassant = 0;
            Castling = Castling.Everything;
            ColorToMove = Color.White;
            MovesCount = 0;
            IrreversibleMovesCount = 0;
            NullMoves = 0;

            CastlingDone[Color.White] = false;
            CastlingDone[Color.Black] = false;

            Material[Color.White] = CalculateMaterial(Color.White);
            Material[Color.Black] = CalculateMaterial(Color.Black);

            Array.Fill(PieceTable, -1);

            CalculatePieceTable(PieceTable);

            Hash = ZobristHashing.CalculateHash(this);
            PawnHash = ZobristHashing.CalculatePawnHash(this);

            MaterialAtOpening = CalculateMaterialAtOpening();

            _killedPieces.Clear();
            _enPassants.Clear();
            _castlings.Clear();
            _promotedPieces.Clear();
            _hashes.Clear();
            _pawnHashes.Clear();
            _irreversibleMovesCounts.Clear();
        }

        public int GetLoudMoves(Span<Move> moves, int offset)
        {
            var movesCount = PawnOperator.GetLoudMoves(this, moves, offset);
            movesCount = KnightOperator.GetLoudMoves(this, moves, movesCount);
            movesCount = BishopOperator.GetLoudMoves(this, moves, movesCount);
            movesCount = RookOperator.GetLoudMoves(this, moves, movesCount);
            movesCount = QueenOperator.GetLoudMoves(this, moves, movesCount);
            movesCount = KingOperator.GetLoudMoves(this, moves, movesCount);

            return movesCount;
        }

        public int GetQuietMoves(Span<Move> moves, int offset)
        {
            var movesCount = PawnOperator.GetQuietMoves(this, moves, offset);
            movesCount = KnightOperator.GetQuietMoves(this, moves, movesCount);
            movesCount = BishopOperator.GetQuietMoves(this, moves, movesCount);
            movesCount = RookOperator.GetQuietMoves(this, moves, movesCount);
            movesCount = QueenOperator.GetQuietMoves(this, moves, movesCount);
            movesCount = KingOperator.GetQuietMoves(this, moves, movesCount);

            return movesCount;
        }

        public int GetAllMoves(Span<Move> moves)
        {
            var movesCount = GetLoudMoves(moves, 0);
            return GetQuietMoves(moves, movesCount);
        }

        public int GetAvailableCaptureMoves(Span<Move> moves)
        {
            var movesCount = PawnOperator.GetAvailableCaptureMoves(this, moves, 0);
            movesCount = KnightOperator.GetAvailableCaptureMoves(this, moves, movesCount);
            movesCount = BishopOperator.GetAvailableCaptureMoves(this, moves, movesCount);
            movesCount = RookOperator.GetAvailableCaptureMoves(this, moves, movesCount);
            movesCount = QueenOperator.GetAvailableCaptureMoves(this, moves, movesCount);
            movesCount = KingOperator.GetAvailableCaptureMoves(this, moves, movesCount);

            return movesCount;
        }

        public bool IsMoveLegal(Move move)
        {
            if (((1ul << move.From) & Occupancy[ColorToMove]) == 0)
            {
                return false;
            }

            var fromPiece = PieceTable[move.From];
            switch (fromPiece)
            {
                case Piece.Pawn: return PawnOperator.IsMoveLegal(this, move);
                case Piece.Knight: return KnightOperator.IsMoveLegal(this, move);
                case Piece.Bishop: return BishopOperator.IsMoveLegal(this, move);
                case Piece.Rook: return RookOperator.IsMoveLegal(this, move);
                case Piece.Queen: return QueenOperator.IsMoveLegal(this, move);
                case Piece.King: return KingOperator.IsMoveLegal(this, move);
            }

            return false;
        }

        public void MakeMove(Move move)
        {
            var pieceType = PieceTable[move.From];
            var enemyColor = ColorOperations.Invert(ColorToMove);

            if (ColorToMove == Color.White)
            {
                MovesCount++;
            }

            _castlings.Push(Castling);
            _hashes.Push(Hash);
            _pawnHashes.Push(PawnHash);
            _enPassants.Push(EnPassant);
            _irreversibleMovesCounts.Push(IrreversibleMovesCount);

            if (pieceType == Piece.Pawn || move.IsCapture())
            {
                IrreversibleMovesCount = 0;
            }
            else
            {
                IrreversibleMovesCount++;
            }

            if (EnPassant != 0)
            {
                var enPassantRank = BitOperations.BitScan(EnPassant) % 8;
                Hash = ZobristHashing.ToggleEnPassant(Hash, enPassantRank);
                EnPassant = 0;
            }

            if (move.Flags == MoveFlags.Quiet)
            {
                MovePiece(ColorToMove, pieceType, move.From, move.To);
                Hash = ZobristHashing.MovePiece(Hash, ColorToMove, pieceType, move.From, move.To);

                if (pieceType == Piece.Pawn)
                {
                    PawnHash = ZobristHashing.MovePiece(PawnHash, ColorToMove, pieceType, move.From, move.To);
                }
            }
            else if (move.Flags == MoveFlags.DoublePush)
            {
                MovePiece(ColorToMove, pieceType, move.From, move.To);
                Hash = ZobristHashing.MovePiece(Hash, ColorToMove, pieceType, move.From, move.To);
                PawnHash = ZobristHashing.MovePiece(PawnHash, ColorToMove, pieceType, move.From, move.To);

                var enPassantField = ColorToMove == Color.White ? 1ul << move.To - 8 : 1ul << move.To + 8;
                var enPassantFieldIndex = BitOperations.BitScan(enPassantField);

                EnPassant |= enPassantField;
                Hash = ZobristHashing.ToggleEnPassant(Hash, enPassantFieldIndex % 8);
            }
            else if (move.Flags == MoveFlags.EnPassant)
            {
                var enemyPieceField = ColorToMove == Color.White ? (byte)(move.To - 8) : (byte)(move.To + 8);
                var killedPiece = PieceTable[enemyPieceField];

                RemovePiece(enemyColor, killedPiece, enemyPieceField);
                Hash = ZobristHashing.AddOrRemovePiece(Hash, enemyColor, killedPiece, enemyPieceField);
                PawnHash = ZobristHashing.AddOrRemovePiece(PawnHash, enemyColor, killedPiece, enemyPieceField);

                MovePiece(ColorToMove, pieceType, move.From, move.To);
                Hash = ZobristHashing.MovePiece(Hash, ColorToMove, pieceType, move.From, move.To);
                PawnHash = ZobristHashing.MovePiece(PawnHash, ColorToMove, pieceType, move.From, move.To);

                _killedPieces.Push(killedPiece);
            }
            else if (move.IsCapture())
            {
                var killedPiece = PieceTable[move.To];

                RemovePiece(enemyColor, killedPiece, move.To);
                Hash = ZobristHashing.AddOrRemovePiece(Hash, enemyColor, killedPiece, move.To);

                if (killedPiece == Piece.Pawn)
                {
                    PawnHash = ZobristHashing.AddOrRemovePiece(PawnHash, enemyColor, killedPiece, move.To);
                }
                else if (killedPiece == Piece.Rook)
                {
                    switch (move.To)
                    {
                        case 0:
                            {
                                Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.WhiteShort);
                                Castling &= ~Castling.WhiteShort;
                                break;
                            }
                        case 7:
                            {
                                Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.WhiteLong);
                                Castling &= ~Castling.WhiteLong;
                                break;
                            }
                        case 56:
                            {
                                Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.BlackShort);
                                Castling &= ~Castling.BlackShort;
                                break;
                            }
                        case 63:
                            {
                                Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.BlackLong);
                                Castling &= ~Castling.BlackLong;
                                break;
                            }
                    }
                }

                // Promotion
                if (((byte)move.Flags & MoveFlagFields.Promotion) != 0)
                {
                    var promotionPiece = GetPromotionPiece(move.Flags);

                    RemovePiece(ColorToMove, pieceType, move.From);
                    Hash = ZobristHashing.AddOrRemovePiece(Hash, ColorToMove, pieceType, move.From);
                    PawnHash = ZobristHashing.AddOrRemovePiece(PawnHash, ColorToMove, pieceType, move.From);

                    AddPiece(ColorToMove, promotionPiece, move.To);
                    Hash = ZobristHashing.AddOrRemovePiece(Hash, ColorToMove, promotionPiece, move.To);

                    _promotedPieces.Push(promotionPiece);
                }
                else
                {
                    MovePiece(ColorToMove, pieceType, move.From, move.To);
                    Hash = ZobristHashing.MovePiece(Hash, ColorToMove, pieceType, move.From, move.To);

                    if (pieceType == Piece.Pawn)
                    {
                        PawnHash = ZobristHashing.MovePiece(PawnHash, ColorToMove, pieceType, move.From, move.To);
                    }
                }

                _killedPieces.Push(killedPiece);
            }
            else if (move.Flags == MoveFlags.KingCastle || move.Flags == MoveFlags.QueenCastle)
            {
                // Short castling
                if (move.Flags == MoveFlags.KingCastle)
                {
                    if (ColorToMove == Color.White)
                    {
                        MovePiece(Color.White, Piece.King, 3, 1);
                        MovePiece(Color.White, Piece.Rook, 0, 2);

                        Hash = ZobristHashing.MovePiece(Hash, Color.White, Piece.King, 3, 1);
                        Hash = ZobristHashing.MovePiece(Hash, Color.White, Piece.Rook, 0, 2);
                    }
                    else
                    {
                        MovePiece(Color.Black, Piece.King, 59, 57);
                        MovePiece(Color.Black, Piece.Rook, 56, 58);

                        Hash = ZobristHashing.MovePiece(Hash, Color.Black, Piece.King, 59, 57);
                        Hash = ZobristHashing.MovePiece(Hash, Color.Black, Piece.Rook, 56, 58);
                    }
                }
                // Long castling
                else
                {
                    if (ColorToMove == Color.White)
                    {
                        MovePiece(Color.White, Piece.King, 3, 5);
                        MovePiece(Color.White, Piece.Rook, 7, 4);

                        Hash = ZobristHashing.MovePiece(Hash, Color.White, Piece.King, 3, 5);
                        Hash = ZobristHashing.MovePiece(Hash, Color.White, Piece.Rook, 7, 4);
                    }
                    else
                    {
                        MovePiece(Color.Black, Piece.King, 59, 61);
                        MovePiece(Color.Black, Piece.Rook, 63, 60);

                        Hash = ZobristHashing.MovePiece(Hash, Color.Black, Piece.King, 59, 61);
                        Hash = ZobristHashing.MovePiece(Hash, Color.Black, Piece.Rook, 63, 60);
                    }
                }

                if (ColorToMove == Color.White)
                {
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.WhiteShort);
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.WhiteLong);
                    Castling &= ~Castling.WhiteCastling;
                }
                else
                {
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.BlackShort);
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.BlackLong);
                    Castling &= ~Castling.BlackCastling;
                }

                CastlingDone[ColorToMove] = true;
            }
            else if (((byte)move.Flags & MoveFlagFields.Promotion) != 0)
            {
                var promotionPiece = GetPromotionPiece(move.Flags);

                RemovePiece(ColorToMove, pieceType, move.From);
                Hash = ZobristHashing.AddOrRemovePiece(Hash, ColorToMove, pieceType, move.From);
                PawnHash = ZobristHashing.AddOrRemovePiece(PawnHash, ColorToMove, pieceType, move.From);

                AddPiece(ColorToMove, promotionPiece, move.To);
                Hash = ZobristHashing.AddOrRemovePiece(Hash, ColorToMove, promotionPiece, move.To);

                _promotedPieces.Push(promotionPiece);
            }

            if (pieceType == Piece.King && move.Flags != MoveFlags.KingCastle && move.Flags != MoveFlags.QueenCastle)
            {
                if (ColorToMove == Color.White)
                {
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.WhiteShort);
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.WhiteLong);
                    Castling &= ~Castling.WhiteCastling;
                }
                else
                {
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.BlackShort);
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.BlackLong);
                    Castling &= ~Castling.BlackCastling;
                }
            }
            else if (pieceType == Piece.Rook && Castling != 0)
            {
                if (move.From == 0)
                {
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.WhiteShort);
                    Castling &= ~Castling.WhiteShort;
                }
                else if (move.From == 7)
                {
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.WhiteLong);
                    Castling &= ~Castling.WhiteLong;
                }
                else if (move.From == 56)
                {
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.BlackShort);
                    Castling &= ~Castling.BlackShort;
                }
                else if (move.From == 63)
                {
                    Hash = ZobristHashing.RemoveCastlingFlag(Hash, Castling, Castling.BlackLong);
                    Castling &= ~Castling.BlackLong;
                }
            }

            ColorToMove = enemyColor;
            Hash = ZobristHashing.ChangeSide(Hash);
        }

        public void UndoMove(Move move)
        {
            var pieceType = PieceTable[move.To];
            ColorToMove = ColorOperations.Invert(ColorToMove);

            if (move.Flags == MoveFlags.Quiet || move.Flags == MoveFlags.DoublePush)
            {
                MovePiece(ColorToMove, pieceType, move.To, move.From);
            }
            else if (move.Flags == MoveFlags.EnPassant)
            {
                var enemyColor = ColorOperations.Invert(ColorToMove);
                var enemyPieceField = ColorToMove == Color.White ? (byte)(move.To - 8) : (byte)(move.To + 8);
                var killedPiece = _killedPieces.Pop();

                MovePiece(ColorToMove, Piece.Pawn, move.To, move.From);
                AddPiece(enemyColor, killedPiece, enemyPieceField);
            }
            else if (move.IsCapture())
            {
                var enemyColor = ColorOperations.Invert(ColorToMove);
                var killedPiece = _killedPieces.Pop();

                // Promotion
                if (((byte)move.Flags & MoveFlagFields.Promotion) != 0)
                {
                    var promotionPiece = _promotedPieces.Pop();
                    RemovePiece(ColorToMove, promotionPiece, move.To);
                    AddPiece(ColorToMove, Piece.Pawn, move.From);
                }
                else
                {
                    MovePiece(ColorToMove, pieceType, move.To, move.From);
                }

                AddPiece(enemyColor, killedPiece, move.To);
            }
            else if (move.Flags == MoveFlags.KingCastle || move.Flags == MoveFlags.QueenCastle)
            {
                // Short castling
                if (move.Flags == MoveFlags.KingCastle)
                {
                    if (ColorToMove == Color.White)
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
                    if (ColorToMove == Color.White)
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

                CastlingDone[ColorToMove] = false;
            }
            else if (((byte)move.Flags & MoveFlagFields.Promotion) != 0)
            {
                var promotionPiece = _promotedPieces.Pop();
                RemovePiece(ColorToMove, promotionPiece, move.To);
                AddPiece(ColorToMove, Piece.Pawn, move.From);
            }

            IrreversibleMovesCount = _irreversibleMovesCounts.Pop();
            PawnHash = _pawnHashes.Pop();
            Hash = _hashes.Pop();
            Castling = _castlings.Pop();
            EnPassant = _enPassants.Pop();

            if (ColorToMove == Color.White)
            {
                MovesCount--;
            }
        }

        public void MakeNullMove()
        {
            NullMoves++;
            if (ColorToMove == Color.White)
            {
                MovesCount++;
            }

            _enPassants.Push(EnPassant);
            _hashes.Push(Hash);

            if (EnPassant != 0)
            {
                var enPassantRank = BitOperations.BitScan(EnPassant) % 8;
                Hash = ZobristHashing.ToggleEnPassant(Hash, enPassantRank);
                EnPassant = 0;
            }

            ColorToMove = ColorOperations.Invert(ColorToMove);
            Hash = ZobristHashing.ChangeSide(Hash);
        }

        public void UndoNullMove()
        {
            NullMoves--;
            ColorToMove = ColorOperations.Invert(ColorToMove);

            Hash = _hashes.Pop();
            EnPassant = _enPassants.Pop();

            if (ColorToMove == Color.White)
            {
                MovesCount--;
            }
        }

        public bool IsFieldAttacked(int color, int fieldIndex)
        {
            var enemyColor = ColorOperations.Invert(color);

            var fileRankAttacks = RookMovesGenerator.GetMoves(OccupancySummary, fieldIndex) & Occupancy[enemyColor];
            var attackingRooks = fileRankAttacks & (Pieces[enemyColor][Piece.Rook] | Pieces[enemyColor][Piece.Queen]);
            if (attackingRooks != 0)
            {
                return true;
            }

            var diagonalAttacks = BishopMovesGenerator.GetMoves(OccupancySummary, fieldIndex) & Occupancy[enemyColor];
            var attackingBishops = diagonalAttacks & (Pieces[enemyColor][Piece.Bishop] | Pieces[enemyColor][Piece.Queen]);
            if (attackingBishops != 0)
            {
                return true;
            }

            var jumpAttacks = KnightMovesGenerator.GetMoves(fieldIndex);
            var attackingKnights = jumpAttacks & Pieces[enemyColor][Piece.Knight];
            if (attackingKnights != 0)
            {
                return true;
            }

            var boxAttacks = KingMovesGenerator.GetMoves(fieldIndex);
            var attackingKings = boxAttacks & Pieces[enemyColor][Piece.King];
            if (attackingKings != 0)
            {
                return true;
            }

            var field = 1ul << fieldIndex;
            var potentialPawns = boxAttacks & Pieces[enemyColor][Piece.Pawn];
            var attackingPawns = color == Color.White ?
                field & ((potentialPawns >> 7) | (potentialPawns >> 9)) :
                field & ((potentialPawns << 7) | (potentialPawns << 9));

            if (attackingPawns != 0)
            {
                return true;
            }

            return false;
        }

        public byte GetAttackingPiecesWithColor(int color, int fieldIndex)
        {
            byte result = 0;

            var jumpAttacks = KnightMovesGenerator.GetMoves(fieldIndex);
            var attackingKnights = jumpAttacks & Pieces[color][Piece.Knight];
            var attackingKnightsCount = BitOperations.Count(attackingKnights);
            if (attackingKnightsCount != 0)
            {
                result |= (byte)((attackingKnightsCount == 1 ? 1 : 3) << SeePiece.Knight1);
            }

            var diagonalAttacks = BishopMovesGenerator.GetMoves(OccupancySummary, fieldIndex) & Occupancy[color];
            var attackingBishops = diagonalAttacks & Pieces[color][Piece.Bishop];
            if (attackingBishops != 0)
            {
                result |= 1 << SeePiece.Bishop;
            }

            var occupancyWithoutFileRantPieces = OccupancySummary & ~Pieces[color][Piece.Rook] & ~Pieces[color][Piece.Queen];
            var fileRankAttacks = RookMovesGenerator.GetMoves(occupancyWithoutFileRantPieces, fieldIndex) & Occupancy[color];
            var attackingRooks = fileRankAttacks & Pieces[color][Piece.Rook];
            var attackingRooksCount = BitOperations.Count(attackingRooks);
            if (attackingRooksCount != 0)
            {
                result |= (byte)((attackingRooksCount == 1 ? 1 : 3) << SeePiece.Rook1);
            }

            var attackingQueens = (fileRankAttacks | diagonalAttacks) & Pieces[color][Piece.Queen];
            if (attackingQueens != 0)
            {
                result |= 1 << SeePiece.Queen;
            }

            var boxAttacks = KingMovesGenerator.GetMoves(fieldIndex);
            var attackingKings = boxAttacks & Pieces[color][Piece.King];
            if (attackingKings != 0)
            {
                result |= 1 << SeePiece.King;
            }

            var field = 1ul << fieldIndex;
            var potentialPawns = boxAttacks & Pieces[color][Piece.Pawn];
            var attackingPawns = color == Color.White ?
                field & ((potentialPawns << 7) | (potentialPawns << 9)) :
                field & ((potentialPawns >> 7) | (potentialPawns >> 9));

            if (attackingPawns != 0)
            {
                result |= 1 << SeePiece.Pawn;
            }

            return result;
        }

        public bool IsKingChecked(int color)
        {
            var king = Pieces[color][Piece.King];
            if (king == 0)
            {
                return false;
            }

            var kingField = BitOperations.BitScan(king);
            return IsFieldAttacked(color, (byte)kingField);
        }

        public void MovePiece(int color, int piece, int from, int to)
        {
            var move = (1ul << from) | (1ul << to);

            Pieces[color][piece] ^= move;
            Occupancy[color] ^= move;
            OccupancySummary ^= move;

            PieceTable[from] = -1;
            PieceTable[to] = piece;
        }

        public void AddPiece(int color, int piece, int fieldIndex)
        {
            var field = 1ul << fieldIndex;

            Pieces[color][piece] ^= field;
            Occupancy[color] ^= field;
            OccupancySummary ^= field;

            Material[color] += EvaluationConstants.Pieces[piece];
            PieceTable[fieldIndex] = piece;
        }

        public void RemovePiece(int color, int piece, int fieldIndex)
        {
            var field = 1ul << fieldIndex;

            Pieces[color][piece] ^= field;
            Occupancy[color] ^= field;
            OccupancySummary ^= field;

            Material[color] -= EvaluationConstants.Pieces[piece];
            PieceTable[fieldIndex] = -1;
        }

        public int CalculateMaterial(int color)
        {
            var material = 0;

            for (var i = 0; i < 6; i++)
            {
                material += (int)BitOperations.Count(Pieces[color][i]) * EvaluationConstants.Pieces[i];
            }

            return material;
        }

        public int GetPhaseRatio()
        {
            var materialOfWeakerSide = Math.Min(Material[Color.White], Material[Color.Black]);

            var openingDelta = MaterialAtOpening - EvaluationConstants.OpeningEndgameEdge;
            var boardDelta = materialOfWeakerSide - EvaluationConstants.OpeningEndgameEdge;

            return Math.Max(boardDelta, 0) * BoardConstants.PhaseResolution / openingDelta;
        }

        public int GetGamePhase()
        {
            var materialOfWeakerSide = Math.Min(Material[Color.White], Material[Color.Black]);
            return materialOfWeakerSide > EvaluationConstants.OpeningEndgameEdge ? GamePhase.Opening : GamePhase.Ending;
        }

        public bool IsThreefoldRepetition()
        {
            if (NullMoves == 0 && _hashes.Count() >= 8)
            {
                var first = _hashes.Peek(3);
                var second = _hashes.Peek(7);

                if (Hash == first && first == second)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsFiftyMoveRuleDraw()
        {
            if (NullMoves == 0 && IrreversibleMovesCount >= 100)
            {
                return true;
            }

            return false;
        }

        public bool IsInsufficientMaterial()
        {
            var drawEdge = EvaluationConstants.Pieces[Piece.King] + 4 * EvaluationConstants.Pieces[Piece.Pawn];
            if (Material[Color.White] < drawEdge && Material[Color.Black] < drawEdge)
            {
                if (Pieces[Color.White][Piece.Pawn] == 0 && Pieces[Color.Black][Piece.Pawn] == 0)
                {
                    return true;
                }
            }

            return false;
        }

        public void CalculatePieceTable(int[] pieceTable)
        {
            for (var fieldIndex = 0; fieldIndex < 64; fieldIndex++)
            {
                for (var pieceIndex = 0; pieceIndex < 6; pieceIndex++)
                {
                    var bitboard = Pieces[Color.White][pieceIndex] | Pieces[Color.Black][pieceIndex];
                    pieceTable[fieldIndex] = -1;

                    if ((bitboard & (1ul << fieldIndex)) != 0)
                    {
                        pieceTable[fieldIndex] = pieceIndex;
                        break;
                    }
                }
            }
        }

        public int CalculateMaterialAtOpening()
        {
            return EvaluationConstants.Pieces[Piece.King] +
                   EvaluationConstants.Pieces[Piece.Queen] +
                   EvaluationConstants.Pieces[Piece.Rook] * 2 +
                   EvaluationConstants.Pieces[Piece.Bishop] * 2 +
                   EvaluationConstants.Pieces[Piece.Knight] * 2 +
                   EvaluationConstants.Pieces[Piece.Pawn] * 8;
        }

        public bool IsFieldPassing(int color, int field)
        {
            var enemyColor = ColorOperations.Invert(color);
            var passingArea = PawnOperator.GetPassingArea(color, field);

            return (passingArea & Pieces[enemyColor][Piece.Pawn]) == 0;
        }

        public override string ToString()
        {
            return BoardToFen.Parse(this);
        }

        private int GetPromotionPiece(MoveFlags flags)
        {
            switch (flags)
            {
                case MoveFlags.QueenPromotion:
                case MoveFlags.QueenPromotionCapture:
                {
                    return Piece.Queen;
                }

                case MoveFlags.RookPromotion:
                case MoveFlags.RookPromotionCapture:
                {
                    return Piece.Rook;
                }

                case MoveFlags.BishopPromotion:
                case MoveFlags.BishopPromotionCapture:
                {
                    return Piece.Bishop;
                }

                case MoveFlags.KnightPromotion:
                case MoveFlags.KnightPromotionCapture:
                {
                    return Piece.Knight;
                }
            }

            throw new InvalidOperationException();
        }
    }
}
