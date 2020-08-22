using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Cosette.Engine.Common
{
    public static class BitOperations
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetLsb(ulong value)
        {
            return Bmi1.X64.ExtractLowestSetBit(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong PopLsb(ulong value)
        {
            return Bmi1.X64.ResetLowestSetBit(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Count(ulong value)
        {
            return Popcnt.X64.PopCount(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BitScan(ulong value)
        {
            return (int) (Lzcnt.X64.LeadingZeroCount(value) ^ 63);
        }
    }
}
