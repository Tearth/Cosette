using System;

namespace Cosette.Engine.Common.Extensions
{
    public static class RandomExtension
    {
        public static ulong NextLong(this Random random)
        {
            var bytes = new byte[8];
            random.NextBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }
    }
}
