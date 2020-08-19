using Cosette.Engine.Common;

namespace Cosette.Engine.Moves.Magic
{
    public static class PermutationsGenerator
    {
        public static ulong GetPermutation(ulong mask, int permutationIndex)
        {
            ulong permutation = 0;
            while (mask != 0)
            {
                var lsb = BitOperations.GetLsb(mask);
                mask = BitOperations.PopLsb(mask);

                if ((permutationIndex & 1) != 0)
                {
                    var lsbIndex = BitOperations.BitScan(lsb);
                    permutation |= 1ul << lsbIndex;
                }

                permutationIndex >>= 1;
            }

            return permutation;
		}
    }
}
