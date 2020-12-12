namespace Cosette.Polyglot.Book
{
    public struct PolyglotBookEntry
    {
        public ulong Hash;
        public PolyglotBookMove Move;
        public ushort Weight;
        public int Learn;

        public static PolyglotBookEntry Zero = new PolyglotBookEntry();

        public override string ToString()
        {
            return Move.ToString();
        }
    }
}
