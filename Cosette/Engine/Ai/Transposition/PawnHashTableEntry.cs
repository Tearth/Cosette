using System.Runtime.InteropServices;

namespace Cosette.Engine.Ai.Transposition
{
    public struct PawnHashTableEntry
    {
        public ushort Key { get; set; }
        public short OpeningScore { get; set; }
        public short EndingScore { get; set; }

        public PawnHashTableEntry(ulong hash, short openingScore, short endingScore)
        {
            Key = (ushort)(hash >> 48);
            OpeningScore = openingScore;
            EndingScore = endingScore;
        }

        public bool IsKeyValid(ulong hash)
        {
            return Key == hash >> 48;
        }
    }
}
