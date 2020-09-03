using System.Runtime.InteropServices;

namespace Cosette.Engine.Ai.Transposition
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PawnHashTableEntry
    {
        public ulong Hash { get; set; }
        public short Score { get; set; }

        public PawnHashTableEntry(ulong hash, short score)
        {
            Hash = hash;
            Score = score;
        }
    }
}
