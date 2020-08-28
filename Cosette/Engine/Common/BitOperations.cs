using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Cosette.Engine.Common
{
    public static class BitOperations
    {
#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ulong GetLsb(ulong value)
        {
            return Bmi1.X64.ExtractLowestSetBit(value);
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ulong PopLsb(ulong value)
        {
            return Bmi1.X64.ResetLowestSetBit(value);
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ulong Count(ulong value)
        {
            return Popcnt.X64.PopCount(value);
        }

#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int BitScan(ulong value)
        {
            return (int) Bmi1.X64.TrailingZeroCount(value);
        }
    }
}
