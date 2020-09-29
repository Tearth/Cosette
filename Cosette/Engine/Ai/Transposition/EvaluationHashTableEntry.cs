using System.Runtime.InteropServices;

namespace Cosette.Engine.Ai.Transposition
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EvaluationHashTableEntry
    {
        public uint Key { get; set; }
        public short Score { get; set; }

        public EvaluationHashTableEntry(ulong hash, short score)
        {
            Key = (uint)(hash >> 32);
            Score = score;
        }

        public bool IsKeyValid(ulong hash)
        {
            return Key == (uint)(hash >> 32);
        }
    }
}
