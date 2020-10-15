using System;

namespace Cosette.Engine.Common.Extensions
{
    public static class RandomExtension
    {
        public static unsafe ulong NextLong(this Random random)
        {
            Span<byte> bytes = stackalloc byte[8];
            random.NextBytes(bytes);

            return BitConverter.ToUInt64(bytes);
        }
    }
}
