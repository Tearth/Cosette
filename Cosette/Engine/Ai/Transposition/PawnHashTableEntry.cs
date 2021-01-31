namespace Cosette.Engine.Ai.Transposition
{
    public struct PawnHashTableEntry
    {
        public ushort Key => (ushort)(_data >> 20);
        public short EndingScore
        {
            get
            {
                var result = (short)((_data >> 10) & 0x3FF);
                return result < 512 ? result : (short)(result - 1024);
            }
        }
        public short OpeningScore
        {
            get
            {
                var result = (short)(_data & 0x3FF);
                return result < 512 ? result : (short)(result - 1024);
            }
        }

        private uint _data;

        public PawnHashTableEntry(ulong hash, short openingScore, short endingScore)
        {
            var hashPart = (uint)((hash >> 52) << 20);
            var endingScorePart = (uint)((endingScore & 0x3FF) << 10);
            var openingScorePart = (uint)(openingScore & 0x3FF);

            _data = hashPart | endingScorePart | openingScorePart;
        }

        public bool IsKeyValid(ulong hash)
        {
            return Key == hash >> 52;
        }
    }
}
