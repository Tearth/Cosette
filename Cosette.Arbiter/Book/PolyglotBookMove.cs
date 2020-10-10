namespace Cosette.Arbiter.Book
{
    public struct PolyglotBookMove
    {
        public byte ToFile => (byte)(_data & 0x7);
        public byte ToRank => (byte)((_data >> 3) & 0x7);
        public byte FromFile => (byte)((_data >> 6) & 0x7);
        public byte FromRank => (byte)((_data >> 9) & 0x7);
        public byte PromotionPiece => (byte)((_data >> 12) & 0x7);

        private ushort _data;

        public override string ToString()
        {
            var from = $"{(char)(FromFile + 'a')}{(char)(FromRank + '1')}";
            var to = $"{(char)(ToFile + 'a')}{(char)(ToRank + '1')}";

            return from + to;
        }
    }
}
