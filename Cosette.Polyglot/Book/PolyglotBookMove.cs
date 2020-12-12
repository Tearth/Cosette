#pragma warning disable 649

namespace Cosette.Polyglot.Book
{
    public struct PolyglotBookMove
    {
        public byte ToFile => (byte)(_data & 0x7);
        public byte ToRank => (byte)((_data >> 3) & 0x7);
        public byte FromFile => (byte)((_data >> 6) & 0x7);
        public byte FromRank => (byte)((_data >> 9) & 0x7);
        public byte PromotionPiece => (byte)((_data >> 12) & 0x7);

        public static PolyglotBookMove Zero = new PolyglotBookMove();

        private ushort _data;

        public static bool operator ==(PolyglotBookMove a, PolyglotBookMove b)
        {
            return a._data == b._data;
        }

        public static bool operator !=(PolyglotBookMove a, PolyglotBookMove b)
        {
            return a._data != b._data;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return this == (PolyglotBookMove)obj;
        }

        public override int GetHashCode()
        {
            return _data;
        }

        public override string ToString()
        {
            var from = $"{(char)(FromFile + 'a')}{(char)(FromRank + '1')}";
            var to = $"{(char)(ToFile + 'a')}{(char)(ToRank + '1')}";
            var result = from + to;

            if (result == "e1h1")
            {
                return "e1g1";
            }

            if (result == "e8h8")
            {
                return "e8g8";
            }
            
            if (result == "e1a1")
            {
                return "e1c1";
            }

            if (result == "e8a8")
            {
                return "e8c8";
            }

            return result;
        }
    }
}
