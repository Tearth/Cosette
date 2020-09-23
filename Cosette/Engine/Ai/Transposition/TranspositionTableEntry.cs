using System.Runtime.InteropServices;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Transposition
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TranspositionTableEntry
    {
        public uint Key { get; set; }
        public byte Depth { get; set; }
        public short Score { get; set; }
        public byte Age { get; set; }
        public TranspositionTableEntryFlags Flags { get; set; }
        public Move BestMove { get; set; }

        public TranspositionTableEntry(ulong hash, byte depth, short score, byte age, Move bestMove, TranspositionTableEntryFlags flags)
        {
            Key = (uint)(hash >> 32);
            Depth = depth;
            Score = score;
            Age = age;
            BestMove = bestMove;
            Flags = flags;
        }

        public bool IsKeyValid(ulong hash)
        {
            return Key == (uint)(hash >> 32);
        }
    }
}
