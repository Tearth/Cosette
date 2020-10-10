namespace Cosette.Arbiter.Book
{
    public struct PolyglotBookEntry
    {
        public ulong Hash;
        public PolyglotBookMove Move;
        public ushort Weight;
        public int Learn;

        public static PolyglotBookEntry Zero = new PolyglotBookEntry();
    }
}
