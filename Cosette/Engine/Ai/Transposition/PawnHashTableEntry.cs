using System.Runtime.InteropServices;

namespace Cosette.Engine.Ai.Transposition
{
    public struct PawnHashTableEntry
    {
        public ushort Key { get; set; }
        public short Score { get; set; }

        public PawnHashTableEntry(ulong hash, short score)
        {
            Key = (ushort)(hash >> 48);
            Score = score;
        }

        public bool IsKeyValid(ulong hash)
        {
            return Key == hash >> 48;
        }
    }
}
