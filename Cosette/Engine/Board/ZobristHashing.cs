using System;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Common.Extensions;

namespace Cosette.Engine.Board
{
    public static class ZobristHashing
    {
        private static ulong[][][] _fieldHashes;
        private static ulong[] _castlingHashes;
        private static ulong[] _enPassantHashes;
        private static ulong _blackSideHash;
        private static Random _random;

        static ZobristHashing()
        {
            _fieldHashes = new ulong[2][][];
            _castlingHashes = new ulong[16];
            _enPassantHashes = new ulong[8];
            _random = new Random();

            _fieldHashes[(int)Color.White] = new ulong[6][];
            _fieldHashes[(int)Color.Black] = new ulong[6][];

            for (var i = 0; i < 6; i++)
            {
                _fieldHashes[0][i] = new ulong[64];
                _fieldHashes[1][i] = new ulong[64];

                PopulateHashArrays(_fieldHashes[(int)Color.White][i]);
                PopulateHashArrays(_fieldHashes[(int)Color.Black][i]);
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

            result ^= _castlingHashes[(int)board.Castling];

            for (var color = 0; color < 2; color++)
            {
                if (board.EnPassant[color] != 0)
                {
                    var enPassant = board.EnPassant[color];
                    var fieldIndex = BitOperations.BitScan(enPassant);
                    var rank = Position.FromFieldIndex(fieldIndex);

                    result ^= _enPassantHashes[rank.X];
                }
            }

            if (board.ColorToMove == Color.Black)
            {
                result ^= _blackSideHash;
            }

            return result;
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
