using System;

namespace Cosette.Engine.Common
{
    public static class Distance
    {
        public static byte[][] Table;

        static Distance()
        {
            Table = new byte[64][];
            for (var from = 0; from < 64; from++)
            {
                Table[from] = new byte[64];
                for (var to = 0; to < 64; to++)
                {
                    var fromPosition = Position.FromFieldIndex(from);
                    var toPosition = Position.FromFieldIndex(to);

                    Table[from][to] = (byte)Math.Max(Math.Abs(fromPosition.X - toPosition.X), Math.Abs(fromPosition.Y - toPosition.Y));
                }
            }
        }
    }
}
