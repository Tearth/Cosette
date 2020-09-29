using System.Runtime.InteropServices;

namespace Cosette.Engine.Ai.Transposition
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EvaluationHashTableEntry
    {
        public uint Key { get; set; }
        public bool KingChecked { get; set; }

        public EvaluationHashTableEntry(ulong hash, bool kingChecked)
        {
            Key = (uint)(hash >> 32);
            KingChecked = kingChecked;
        }

        public bool IsKeyValid(ulong hash)
        {
            return Key == (uint)(hash >> 32);
        }
    }
}
