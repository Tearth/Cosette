using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai
{
    public struct TranspositionTableEntry
    {
        public byte Key { get; set; }
        public byte Depth { get; set; }
        public short Score { get; set; }
        public Move BestMove { get; set; }
        public TranspositionTableEntryType Type { get; set; }

        public TranspositionTableEntry(byte key, byte depth, short score, Move bestMove, TranspositionTableEntryType type)
        {
            Key = key;
            Depth = depth;
            Score = score;
            BestMove = bestMove;
            Type = type;
        }
    }
}
