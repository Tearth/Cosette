#if BMI
using System.Runtime.Intrinsics.X86;
#endif

namespace Cosette.Engine.Common
{
    public static class BitOperations
    {
#if !BMI
        private static readonly int[] BitScanValues = {
            0,  1,  48,  2, 57, 49, 28,  3,
            61, 58, 50, 42, 38, 29, 17,  4,
            62, 55, 59, 36, 53, 51, 43, 22,
            45, 39, 33, 30, 24, 18, 12,  5,
            63, 47, 56, 27, 60, 41, 37, 16,
            54, 35, 52, 21, 44, 32, 23, 11,
            46, 26, 40, 15, 34, 20, 31, 10,
            25, 14, 19,  9, 13,  8,  7,  6
        };
#endif

        public static ulong GetLsb(ulong value)
        {
#if BMI
            return Bmi1.X64.ExtractLowestSetBit(value);
#else
            return (ulong)((long)value & -(long)value);
#endif
        }

        public static ulong PopLsb(ulong value)
        {
#if BMI
            return Bmi1.X64.ResetLowestSetBit(value);
#else
            return value & (value - 1);
#endif
        }

        public static ulong Count(ulong value)
        {
#if BMI
            return Popcnt.X64.PopCount(value);
#else
            var count = 0ul;
            while (value > 0)
            {
                value = PopLsb(value);
                count++;
            }

            return count;
#endif
        }

        public static int BitScan(ulong value)
        {
#if BMI
            return (int)Bmi1.X64.TrailingZeroCount(value);
#else
            return BitScanValues[((ulong)((long)value & -(long)value) * 0x03f79d71b4cb0a89) >> 58];
#endif
        }
    }
}
