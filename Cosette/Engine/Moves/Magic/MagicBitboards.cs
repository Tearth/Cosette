using System;
using Cosette.Engine.Board;
using Cosette.Engine.Common.Extensions;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Moves.Magic
{
    public static class MagicBitboards
    {
        private static MagicContainer[] _rookMagicArray;
        private static MagicContainer[] _bishopMagicArray;
        private static readonly Random _random;

        static MagicBitboards()
        {
            _random = new Random();
        }

        public static void InitWithInternalKeys()
        {
            GenerateRookAttacks(MagicKeys.RookKeys);
            GenerateBishopAttacks(MagicKeys.BishopKeys);
        }

        public static ulong GetRookMoves(ulong board, int fieldIndex)
        {
            board &= _rookMagicArray[fieldIndex].Mask;
            board *= _rookMagicArray[fieldIndex].MagicNumber;
            board >>= _rookMagicArray[fieldIndex].Shift;
            return _rookMagicArray[fieldIndex].Attacks[board];
        }

        public static ulong GetBishopMoves(ulong board, int fieldIndex)
        {
            board &= _bishopMagicArray[fieldIndex].Mask;
            board *= _bishopMagicArray[fieldIndex].MagicNumber;
            board >>= _bishopMagicArray[fieldIndex].Shift;
            return _bishopMagicArray[fieldIndex].Attacks[board];
        }

        public static MagicContainer[] GenerateRookAttacks(ulong[] keys = null)
        {
            var masks = new ulong[64];
            var permutations = new ulong[64][];
            var attacks = new ulong[64][];

            for (var fieldIndex = 0; fieldIndex < 64; fieldIndex++)
            {
                masks[fieldIndex] = (FilePatternGenerator.GetPattern(fieldIndex) & ~BoardConstants.TopBottomEdge) |
                                    (RankPatternGenerator.GetPattern(fieldIndex) & ~BoardConstants.RightLeftEdge);

                permutations[fieldIndex] = new ulong[1 << MagicShifts.RookShifts[fieldIndex]];
                attacks[fieldIndex] = new ulong[1 << MagicShifts.RookShifts[fieldIndex]];

                for (var permutationIndex = 0; permutationIndex < permutations[fieldIndex].Length; permutationIndex++)
                {
                    permutations[fieldIndex][permutationIndex] = PermutationsGenerator.GetPermutation(masks[fieldIndex], permutationIndex);
                    attacks[fieldIndex][permutationIndex] = AttacksGenerator.GetFileRankAttacks(permutations[fieldIndex][permutationIndex], fieldIndex);
                }
            }

            return _rookMagicArray = GenerateAttacks(masks, permutations, attacks, MagicShifts.RookShifts, keys);
        }

        public static MagicContainer[] GenerateBishopAttacks(ulong[] keys = null)
        {
            var masks = new ulong[64];
            var permutations = new ulong[64][];
            var attacks = new ulong[64][];

            for (var fieldIndex = 0; fieldIndex < 64; fieldIndex++)
            {
                masks[fieldIndex] = DiagonalPatternGenerator.GetPattern(fieldIndex) & ~BoardConstants.Edges;
                permutations[fieldIndex] = new ulong[1 << MagicShifts.BishopShifts[fieldIndex]];
                attacks[fieldIndex] = new ulong[1 << MagicShifts.BishopShifts[fieldIndex]];

                for (var permutationIndex = 0; permutationIndex < permutations[fieldIndex].Length; permutationIndex++)
                {
                    permutations[fieldIndex][permutationIndex] = PermutationsGenerator.GetPermutation(masks[fieldIndex], permutationIndex);
                    attacks[fieldIndex][permutationIndex] = AttacksGenerator.GetDiagonalAttacks(permutations[fieldIndex][permutationIndex], fieldIndex);
                }
            }

            return _bishopMagicArray = GenerateAttacks(masks, permutations, attacks, MagicShifts.BishopShifts, keys);
        }

        private static MagicContainer[] GenerateAttacks(ulong[] masks, ulong[][] permutations, ulong[][] attacks, int[] shifts, ulong[] keys = null)
        {
            var magicArray = new MagicContainer[64];

            for (var fieldIndex = 0; fieldIndex < 64; fieldIndex++)
            {
                magicArray[fieldIndex] = new MagicContainer
                {
                    Shift = 64 - shifts[fieldIndex],
                    Mask = masks[fieldIndex],
                    Attacks = new ulong[1 << shifts[fieldIndex]]
                };

                var success = false;
                while (!success)
                {
                    success = true;
                    magicArray[fieldIndex].MagicNumber = keys?[fieldIndex] ?? GetRandomMagicNumber();

                    for (var permutationIndex = 0; permutationIndex < permutations[fieldIndex].Length; permutationIndex++)
                    {
                        var hash = permutations[fieldIndex][permutationIndex] * magicArray[fieldIndex].MagicNumber;
                        var attackIndex = hash >> magicArray[fieldIndex].Shift;

                        if (magicArray[fieldIndex].Attacks[attackIndex] != 0 && magicArray[fieldIndex].Attacks[attackIndex] != attacks[fieldIndex][permutationIndex])
                        {
                            Array.Clear(magicArray[fieldIndex].Attacks, 0, magicArray[fieldIndex].Attacks.Length);
                            success = false;
                            break;
                        }

                        magicArray[fieldIndex].Attacks[attackIndex] = attacks[fieldIndex][permutationIndex];
                    }
                }
            }

            return magicArray;
        }

        private static ulong GetRandomMagicNumber()
        {
            return _random.NextLong() & _random.NextLong() & _random.NextLong();
        }
    }
}
