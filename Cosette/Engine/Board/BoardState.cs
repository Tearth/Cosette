using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cosette.Engine.Ai;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Score.PieceSquareTables;
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
        public ulong EnPassant { get; set; }
        public Castling Castling { get; set; }
        public Color ColorToMove { get; set; }

        public bool[] CastlingDone { get; set; }
        public int[] Material { get; set; }
        public int[][] Position { get; set; }

        public ulong Hash { get; set; }
        public ulong PawnHash { get; set; }

        private readonly FastStack<Piece> _killedPieces;
        private readonly FastStack<ulong> _enPassants;
        private readonly FastStack<Castling> _castlings;
        private readonly FastStack<Piece> _promotedPieces;
        private readonly FastStack<ulong> _hashes;
        private readonly FastStack<ulong> _pawnHashes;

        private int _materialAtOpening;

        public BoardState()
        {
            Pieces = new ulong[2][];
            Pieces[(int)Color.White] = new ulong[6];
            Pieces[(int)Color.Black] = new ulong[6];

            Occupancy = new ulong[2];
            CastlingDone = new bool[2];
            Material = new int[2];

            Position = new int[2][];
            Position[(int)Color.White] = new int[2];
            Position[(int)Color.Black] = new int[2];

            _killedPieces = new FastStack<Piece>(512);
            _enPassants = new FastStack<ulong>(512);
            _castlings = new FastStack<Castling>(512);
            _promotedPieces = new FastStack<Piece>(512);
            _hashes = new FastStack<ulong>(512);
            _pawnHashes = new FastStack<ulong>(512);

            _materialAtOpening =
                EvaluationConstants.Pieces[(int) Piece.King] +
                EvaluationConstants.Pieces[(int) Piece.Queen] +
                EvaluationConstants.Pieces[(int) Piece.Rook] * 2 +
                EvaluationConstants.Pieces[(int) Piece.Bishop] * 2+
                EvaluationConstants.Pieces[(int) Piece.Knight] * 2+
                EvaluationConstants.Pieces[(int) Piece.Pawn] * 8;
            _materialAtOpening *= 2;
        }

        public void SetDefaultState()
        {
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
            ColorToMove = Color.White;

            CastlingDone[(int)Color.White] = false;
            CastlingDone[(int)Color.Black] = false;

            Material[(int)Color.White] = CalculateMaterial(Color.White);
            Material[(int)Color.Black] = CalculateMaterial(Color.Black);

            Position[(int)Color.White][(int)GamePhase.Opening] = CalculatePosition(Color.White, GamePhase.Opening);
            Position[(int)Color.White][(int)GamePhase.Ending] = CalculatePosition(Color.White, GamePhase.Ending);
            Position[(int)Color.Black][(int)GamePhase.Opening] = CalculatePosition(Color.Black, GamePhase.Opening);
            Position[(int)Color.Black][(int)GamePhase.Ending] = CalculatePosition(Color.Black, GamePhase.Ending);

            Hash = ZobristHashing.CalculateHash(this);
            PawnHash = ZobristHashing.CalculatePawnHash(this);

            _killedPieces.Clear();
            _enPassants.Clear();
            _castlings.Clear();
            _promotedPieces.Clear();
            _hashes.Clear();
            _pawnHashes.Clear();
        }

        public int GetAvailableMoves(Span<Move> moves)
        {
            var movesCount = PawnOperator.GetAvailableMoves(this, ColorToMove, moves, 0);
            movesCount = KnightOperator.GetAvailableMoves(this, ColorToMove, moves, movesCount);
            movesCount = BishopOperator.GetAvailableMoves(this, ColorToMove, moves, movesCount);
            movesCount = RookOperator.GetAvailableMoves(this, ColorToMove, moves, movesCount);
            movesCount = QueenOperator.GetAvailableMoves(this, ColorToMove, moves, movesCount);
            movesCount = KingOperator.GetAvailableMoves(this, ColorToMove, moves, movesCount);

            return movesCount;
        }

        public int GetAvailableQMoves(Span<Move> moves)
        {
            var movesCount = PawnOperator.GetAvailableQMoves(this, ColorToMove, moves, 0);
            movesCount = KnightOperator.GetAvailableQMoves(this, ColorToMove, moves, movesCount);
            movesCount = BishopOperator.GetAvailableQMoves(this, ColorToMove, moves, movesCount);
            movesCount = RookOperator.GetAvailableQMoves(this, ColorToMove, moves, movesCount);
            movesCount = QueenOperator.GetAvailableQMoves(this, ColorToMove, moves, movesCount);
            movesCount = KingOperator.GetAvailableQMoves(this, ColorToMove, moves, movesCount);

            return movesCount;
        }

        public void MakeMove(Move move)
        {
            var enemyColor = ColorOperations.Invert(ColorToMove);

            _castlings.Push(Castling);
            _hashes.Push(Hash);
            _pawnHashes.Push(PawnHash);
            _enPassants.Push(EnPassant);

            if (EnPassant != 0)
            {
                var enPassantRank = BitOperations.BitScan(EnPassant) % 8;
                Hash = ZobristHashing.ToggleEnPassant(Hash, enPassantRank);
                EnPassant = 0;
            }

            if (move.Flags == MoveFlags.None)
            {
                MovePiece(ColorToMove, move.Piece, move.From, move.To);
                Hash = ZobristHashing.MovePiece(Hash, ColorToMove, move.Piece, move.From, move.To);

                if (move.Piece == Piece.Pawn)
                {
                    PawnHash = ZobristHashing.MovePiece(PawnHash, ColorToMove, move.Piece, move.From, move.To);
                }
            }
            else if ((move.Flags & MoveFlags.DoublePush) != 0)
            {
                MovePiece(ColorToMove, move.Piece, move.From, move.To);
                Hash = ZobristHashing.MovePiece(Hash, ColorToMove, move.Piece, move.From, move.To);
                PawnHash = ZobristHashing.MovePiece(PawnHash, ColorToMove, move.Piece, move.From, move.To);

                var enPassantField = ColorToMove == Color.White ? 1ul << move.To - 8 : 1ul << move.To + 8;
                var enPassantFieldIndex = BitOperations.BitScan(enPassantField);

                EnPassant |= enPassantField;
                Hash = ZobristHashing.ToggleEnPassant(Hash, enPassantFieldIndex % 8);
            }
            else if ((move.Flags & MoveFlags.Kill) != 0)
            {
                var killedPiece = GetPiece(enemyColor, move.To);

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
                if ((byte)move.Flags >= 16)
                {
                    var promotionPiece = GetPromotionPiece(move.Flags);

                    RemovePiece(ColorToMove, move.Piece, move.From);
                    Hash = ZobristHashing.AddOrRemovePiece(Hash, ColorToMove, move.Piece, move.From);
                    PawnHash = ZobristHashing.AddOrRemovePiece(PawnHash, ColorToMove, move.Piece, move.From);

                    AddPiece(ColorToMove, promotionPiece, move.To);
                    Hash = ZobristHashing.AddOrRemovePiece(Hash, ColorToMove, promotionPiece, move.To);

                    _promotedPieces.Push(promotionPiece);
                }
                else
                {
                    MovePiece(ColorToMove, move.Piece, move.From, move.To);
                    Hash = ZobristHashing.MovePiece(Hash, ColorToMove, move.Piece, move.From, move.To);

                    if (move.Piece == Piece.Pawn)
                    {
                        PawnHash = ZobristHashing.MovePiece(PawnHash, ColorToMove, move.Piece, move.From, move.To);
                    }
                }

                _killedPieces.Push(killedPiece);
            }
            else if ((move.Flags & MoveFlags.Castling) != 0)
            {
                // Short castling
                if (move.From > move.To)
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

                CastlingDone[(int)ColorToMove] = true;
            }
            else if ((move.Flags & MoveFlags.EnPassant) != 0)
            {
                var enemyPieceField = ColorToMove == Color.White ? (byte)(move.To - 8) : (byte)(move.To + 8);
                var killedPiece = GetPiece(enemyColor, enemyPieceField);

                RemovePiece(enemyColor, killedPiece, enemyPieceField);
                Hash = ZobristHashing.AddOrRemovePiece(Hash, enemyColor, killedPiece, enemyPieceField);
                PawnHash = ZobristHashing.AddOrRemovePiece(PawnHash, enemyColor, killedPiece, enemyPieceField);

                MovePiece(ColorToMove, move.Piece, move.From, move.To);
                Hash = ZobristHashing.MovePiece(Hash, ColorToMove, move.Piece, move.From, move.To);
                PawnHash = ZobristHashing.MovePiece(PawnHash, ColorToMove, move.Piece, move.From, move.To);

                _killedPieces.Push(killedPiece);
            }
            else if ((byte)move.Flags >= 16)
            {
                var promotionPiece = GetPromotionPiece(move.Flags);

                RemovePiece(ColorToMove, move.Piece, move.From);
                Hash = ZobristHashing.AddOrRemovePiece(Hash, ColorToMove, move.Piece, move.From);
                PawnHash = ZobristHashing.AddOrRemovePiece(PawnHash, ColorToMove, move.Piece, move.From);

                AddPiece(ColorToMove, promotionPiece, move.To);
                Hash = ZobristHashing.AddOrRemovePiece(Hash, ColorToMove, promotionPiece, move.To);

                _promotedPieces.Push(promotionPiece);
            }

            if (move.Piece == Piece.King && move.Flags != MoveFlags.Castling)
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
            else if (move.Piece == Piece.Rook && Castling != 0)
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
            ColorToMove = ColorOperations.Invert(ColorToMove);

            if (move.Flags == MoveFlags.None || (move.Flags & MoveFlags.DoublePush) != 0)
            {
                MovePiece(ColorToMove, move.Piece, move.To, move.From);
            }
            else if ((move.Flags & MoveFlags.Kill) != 0)
            {
                var enemyColor = ColorOperations.Invert(ColorToMove);
                var killedPiece = _killedPieces.Pop();

                // Promotion
                if ((byte)move.Flags >= 16)
                {
                    var promotionPiece = _promotedPieces.Pop();
                    RemovePiece(ColorToMove, promotionPiece, move.To);
                    AddPiece(ColorToMove, move.Piece, move.From);
                }
                else
                {
                    MovePiece(ColorToMove, move.Piece, move.To, move.From);
                }

                AddPiece(enemyColor, killedPiece, move.To);
            }
            else if ((move.Flags & MoveFlags.Castling) != 0)
            {
                // Short castling
                if (move.From > move.To)
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

                CastlingDone[(int)ColorToMove] = false;
            }
            else if ((move.Flags & MoveFlags.EnPassant) != 0)
            {
                var enemyColor = ColorOperations.Invert(ColorToMove);
                var enemyPieceField = ColorToMove == Color.White ? (byte)(move.To - 8) : (byte)(move.To + 8);
                var killedPiece = _killedPieces.Pop();

                MovePiece(ColorToMove, move.Piece, move.To, move.From);
                AddPiece(enemyColor, killedPiece, enemyPieceField);
            }
            else if ((byte)move.Flags >= 16)
            {
                var promotionPiece = _promotedPieces.Pop();
                RemovePiece(ColorToMove, promotionPiece, move.To);
                AddPiece(ColorToMove, move.Piece, move.From);
            }

            PawnHash = _pawnHashes.Pop();
            Hash = _hashes.Pop();
            Castling = _castlings.Pop();
            EnPassant = _enPassants.Pop();
        }

        public void MakeNullMove()
        {
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
            ColorToMove = ColorOperations.Invert(ColorToMove);

            Hash = _hashes.Pop();
            EnPassant = _enPassants.Pop();
        }

        public bool IsFieldAttacked(Color color, byte fieldIndex)
        {
            var enemyColor = ColorOperations.Invert(color);

            var fileRankAttacks = RookMovesGenerator.GetMoves(OccupancySummary, fieldIndex) & Occupancy[(int)enemyColor];
            var attackingRooks = fileRankAttacks & Pieces[(int)enemyColor][(int)Piece.Rook];
            if (attackingRooks != 0)
            {
                return true;
            }

            var diagonalAttacks = BishopMovesGenerator.GetMoves(OccupancySummary, fieldIndex) & Occupancy[(int)enemyColor];
            var attackingBishops = diagonalAttacks & Pieces[(int)enemyColor][(int)Piece.Bishop];
            if (attackingBishops != 0)
            {
                return true;
            }

            var attackingQueens = (fileRankAttacks | diagonalAttacks) & Pieces[(int)enemyColor][(int)Piece.Queen];
            if (attackingQueens != 0)
            {
                return true;
            }

            var jumpAttacks = KnightMovesGenerator.GetMoves(fieldIndex);
            var attackingKnights = jumpAttacks & Pieces[(int)enemyColor][(int)Piece.Knight];
            if (attackingKnights != 0)
            {
                return true;
            }

            var boxAttacks = KingMovesGenerator.GetMoves(fieldIndex);
            var attackingKings = boxAttacks & Pieces[(int)enemyColor][(int)Piece.King];
            if (attackingKings != 0)
            {
                return true;
            }

            var field = 1ul << fieldIndex;
            var potentialPawns = boxAttacks & Pieces[(int)enemyColor][(int)Piece.Pawn];
            var attackingPawns = color == Color.White ?
                field & ((potentialPawns >> 7) | (potentialPawns >> 9)) :
                field & ((potentialPawns << 7) | (potentialPawns << 9));

            if (attackingPawns != 0)
            {
                return true;
            }

            return false;
        }

        public byte GetAttackingPiecesWithColor(Color color, byte fieldIndex)
        {
            byte result = 0;

            var fileRankAttacks = RookMovesGenerator.GetMoves(OccupancySummary, fieldIndex) & Occupancy[(int)color];
            var attackingRooks = fileRankAttacks & Pieces[(int)color][(int)Piece.Rook];
            if (attackingRooks != 0)
            {
                result |= 1 << (int) Piece.Rook;
            }

            var diagonalAttacks = BishopMovesGenerator.GetMoves(OccupancySummary, fieldIndex) & Occupancy[(int)color];
            var attackingBishops = diagonalAttacks & Pieces[(int)color][(int)Piece.Bishop];
            if (attackingBishops != 0)
            {
                result |= 1 << (int)Piece.Bishop;
            }

            var attackingQueens = (fileRankAttacks | diagonalAttacks) & Pieces[(int)color][(int)Piece.Queen];
            if (attackingQueens != 0)
            {
                result |= 1 << (int)Piece.Queen;
            }

            var jumpAttacks = KnightMovesGenerator.GetMoves(fieldIndex);
            var attackingKnights = jumpAttacks & Pieces[(int)color][(int)Piece.Knight];
            if (attackingKnights != 0)
            {
                result |= 1 << (int)Piece.Knight;
            }

            var boxAttacks = KingMovesGenerator.GetMoves(fieldIndex);
            var attackingKings = boxAttacks & Pieces[(int)color][(int)Piece.King];
            if (attackingKings != 0)
            {
                result |= 1 << (int)Piece.King;
            }

            var field = 1ul << fieldIndex;
            var potentialPawns = boxAttacks & Pieces[(int)color][(int)Piece.Pawn];
            var attackingPawns = color == Color.White ?
                field & ((potentialPawns << 7) | (potentialPawns << 9)) :
                field & ((potentialPawns >> 7) | (potentialPawns >> 9));

            if (attackingPawns != 0)
            {
                result |= 1 << (int)Piece.Pawn;
            }

            return result;
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public bool IsKingChecked(Color color)
        {
            var king = Pieces[(int) color][(int) Piece.King];
            var kingField = BitOperations.BitScan(king);

            return IsFieldAttacked(color, (byte)kingField);
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void MovePiece(Color color, Piece piece, byte from, byte to)
        {
            var move = (1ul << from) | (1ul << to);

            Pieces[(int) color][(int) piece] ^= move;
            Occupancy[(int)color] ^= move;
            OccupancySummary ^= move;
            
            Position[(int)color][(int)GamePhase.Opening] -= PieceSquareTablesData.Values[(int)piece][(int)color][(int)GamePhase.Opening][from];
            Position[(int)color][(int)GamePhase.Opening] += PieceSquareTablesData.Values[(int)piece][(int)color][(int)GamePhase.Opening][to];

            Position[(int)color][(int)GamePhase.Ending] -= PieceSquareTablesData.Values[(int)piece][(int)color][(int)GamePhase.Ending][from];
            Position[(int)color][(int)GamePhase.Ending] += PieceSquareTablesData.Values[(int)piece][(int)color][(int)GamePhase.Ending][to];
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public Piece GetPiece(Color color, byte from)
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

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void AddPiece(Color color, Piece piece, byte at)
        {
            var field = 1ul << at;

            Pieces[(int)color][(int)piece] ^= field;
            Occupancy[(int)color] ^= field;
            OccupancySummary ^= field;

            Material[(int)color] += EvaluationConstants.Pieces[(int)piece];

            Position[(int)color][(int)GamePhase.Opening] += PieceSquareTablesData.Values[(int)piece][(int)color][(int)GamePhase.Opening][at];
            Position[(int)color][(int)GamePhase.Ending] += PieceSquareTablesData.Values[(int)piece][(int)color][(int)GamePhase.Ending][at];
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public void RemovePiece(Color color, Piece piece, byte at)
        {
            var field = 1ul << at;

            Pieces[(int)color][(int)piece] ^= field;
            Occupancy[(int)color] ^= field;
            OccupancySummary ^= field;

            Material[(int)color] -= EvaluationConstants.Pieces[(int)piece];

            Position[(int)color][(int)GamePhase.Opening] -= PieceSquareTablesData.Values[(int)piece][(int)color][(int)GamePhase.Opening][at];
            Position[(int)color][(int)GamePhase.Ending] -= PieceSquareTablesData.Values[(int)piece][(int)color][(int)GamePhase.Ending][at];
        }

        public int CalculateMaterial(Color color)
        {
            var material = 0;

            for (var i = 0; i < 6; i++)
            {
                material += (int)BitOperations.Count(Pieces[(int)color][i]) * EvaluationConstants.Pieces[i];
            }

            return material;
        }

        public int CalculatePosition(Color color, GamePhase phase)
        {
            var result = 0;

            for (var pieceIndex = 0; pieceIndex < 6; pieceIndex++)
            {
                var pieces = Pieces[(int)color][pieceIndex];
                while (pieces != 0)
                {
                    var lsb = BitOperations.GetLsb(pieces);
                    pieces = BitOperations.PopLsb(pieces);
                    var fieldIndex = BitOperations.BitScan(lsb);

                    result += PieceSquareTablesData.Values[pieceIndex][(int)color][(int)phase][fieldIndex];
                }
            }

            return result;
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public float GetPhaseRatio()
        {
            var openingDelta = _materialAtOpening - EvaluationConstants.OpeningEndgameEdge;
            var boardDelta = Material[(int) Color.White] + Material[(int) Color.Black] - EvaluationConstants.OpeningEndgameEdge;
            var ratio = (float) boardDelta / openingDelta;

            return Math.Max(0, ratio);
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public GamePhase GetGamePhase()
        {
            var totalMaterial = Material[(int) Color.White] + Material[(int) Color.Black];
            return totalMaterial > EvaluationConstants.OpeningEndgameEdge ? GamePhase.Opening : GamePhase.Ending;
        }

        public bool IsThreefoldRepetition()
        {
            if (_hashes.Count() >= 8)
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

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
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
