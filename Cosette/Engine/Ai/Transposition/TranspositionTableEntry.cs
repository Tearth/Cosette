using System.Runtime.InteropServices;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Transposition
{
    public struct TranspositionTableEntry
    {
        public ushort Key { get; set; }
        public short Score { get; set; }
        public Move BestMove { get; set; }
        public byte Depth { get; set; }

        public byte Age => (byte)(_ageFlagsField >> 3);
        public TranspositionTableEntryFlags Flags => (TranspositionTableEntryFlags)(_ageFlagsField & 7);

        private byte _ageFlagsField;

        public TranspositionTableEntry(ulong hash, short score, Move bestMove, byte depth, TranspositionTableEntryFlags flags, byte age)
        {
            Key = (ushort)(hash >> 48);
            Score = score;
            BestMove = bestMove;
            Depth = depth;

            _ageFlagsField = (byte)flags;
            _ageFlagsField |= (byte)(age << 3);
        }

        public bool IsKeyValid(ulong hash)
        {
            return Key == (ushort)(hash >> 48);
        }
    }
}
