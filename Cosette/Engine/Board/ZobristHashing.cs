using System;
using Cosette.Engine.Common;
using Cosette.Engine.Common.Extensions;

namespace Cosette.Engine.Board
{
    public static class ZobristHashing
    {
        private static readonly ulong[][][] _fieldHashes;
        private static readonly ulong[] _castlingHashes;
        private static readonly ulong[] _enPassantHashes;
        private static readonly ulong _blackSideHash;
        private static readonly Random _random;

        static ZobristHashing()
        {
            _fieldHashes = new ulong[2][][];
            _castlingHashes = new ulong[4];
            _enPassantHashes = new ulong[8];
            _random = new Random();

            _fieldHashes[Color.White] = new ulong[6][];
            _fieldHashes[Color.Black] = new ulong[6][];

            for (var piece = 0; piece < 6; piece++)
            {
                _fieldHashes[0][piece] = new ulong[64];
                _fieldHashes[1][piece] = new ulong[64];

                PopulateHashArrays(_fieldHashes[Color.White][piece]);
                PopulateHashArrays(_fieldHashes[Color.Black][piece]);
            }

            PopulateHashArrays(_castlingHashes);
            PopulateHashArrays(_enPassantHashes);
            _blackSideHash = _random.NextLong();
        }

        public static ulong CalculateHash(BoardState board)
        {
            var result = 0ul;
            for (var color = 0; color < 2; color++)
            {
                for (var piece = 0; piece < 6; piece++)
                {
                    var piecesToParse = board.Pieces[color][piece];
                    while (piecesToParse != 0)
                    {
                        var lsb = BitOperations.GetLsb(piecesToParse);
                        piecesToParse = BitOperations.PopLsb(piecesToParse);

                        var fieldIndex = BitOperations.BitScan(lsb);
                        result ^= _fieldHashes[color][piece][fieldIndex];
                    }
                }
            }
            
            if ((board.Castling & Castling.WhiteShort) != 0)
            {
                result ^= _castlingHashes[0];
            }
            if ((board.Castling & Castling.WhiteLong) != 0)
            {
                result ^= _castlingHashes[1];
            }
            if ((board.Castling & Castling.BlackShort) != 0)
            {
                result ^= _castlingHashes[2];
            }
            if ((board.Castling & Castling.BlackLong) != 0)
            {
                result ^= _castlingHashes[3];
            }
            
            if (board.EnPassant != 0)
            {
                var fieldIndex = BitOperations.BitScan(board.EnPassant);
                result ^= _enPassantHashes[fieldIndex % 8];
            }
            
            if (board.ColorToMove == Color.Black)
            {
                result ^= _blackSideHash;
            }
            
            return result;
        }

        public static ulong CalculatePawnHash(BoardState board)
        {
            var result = 0ul;
            for (var color = 0; color < 2; color++)
            {
                var piecesToParse = board.Pieces[color][Piece.Pawn];
                while (piecesToParse != 0)
                {
                    var lsb = BitOperations.GetLsb(piecesToParse);
                    piecesToParse = BitOperations.PopLsb(piecesToParse);

                    var fieldIndex = BitOperations.BitScan(lsb);
                    result ^= _fieldHashes[color][Piece.Pawn][fieldIndex];
                }
            }

            return result;
        }

        public static ulong MovePiece(ulong hash, int color, int piece, byte from, byte to)
        {
            return hash ^ _fieldHashes[color][piece][from] ^ _fieldHashes[color][piece][to];
        }

        public static ulong AddOrRemovePiece(ulong hash, int color, int piece, byte at)
        {
            return hash ^ _fieldHashes[color][piece][at];
        }

        public static ulong RemoveCastlingFlag(ulong hash, Castling currentCastling, Castling castlingChange)
        {
            if ((currentCastling & castlingChange) != 0)
            {
                return hash ^ _castlingHashes[BitOperations.BitScan((ulong)castlingChange)];
            }

            return hash;
        }

        public static ulong ToggleEnPassant(ulong hash, int enPassantRank)
        {
            return hash ^ _enPassantHashes[enPassantRank];
        }

        public static ulong ChangeSide(ulong hash)
        {
            return hash ^ _blackSideHash;
        }

        private static void PopulateHashArrays(ulong[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = _random.NextLong();
            }
        }
    }
}
