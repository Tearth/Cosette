using System.Runtime.InteropServices;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Transposition
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TranspositionTableEntry
    {
        public ulong Hash { get; set; }
        public short Score { get; set; }
        public byte Depth { get; set; }
        public TranspositionTableEntryType Type { get; set; }
        public Move BestMove { get; set; }

        public TranspositionTableEntry(ulong hash, byte depth, short score, Move bestMove, TranspositionTableEntryType type)
        {
            Hash = hash;
            Depth = depth;
            Score = score;
            BestMove = bestMove;
            Type = type;
        }
    }
}
