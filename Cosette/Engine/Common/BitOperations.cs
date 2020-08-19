using System.Runtime.Intrinsics.X86;

namespace Cosette.Engine.Common
{
    public static class BitOperations
    {
        public static ulong GetLsb(ulong value)
        {
            return Bmi1.X64.ExtractLowestSetBit(value);
        }

        public static ulong PopLsb(ulong value)
        {
            return Bmi1.X64.ResetLowestSetBit(value);
        }

        public static ulong Count(ulong value)
        {
            return Popcnt.X64.PopCount(value);
        }

        public static int BitScan(ulong value)
        {
            return (int) (Lzcnt.X64.LeadingZeroCount(value) ^ 63);
        }
    }
}
