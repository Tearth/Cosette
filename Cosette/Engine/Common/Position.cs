namespace Cosette.Engine.Common
{
    public readonly struct Position
    {
        public readonly int X;
        public readonly int Y;

        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool IsValid()
        {
            return X >= 0 && X <= 7 && Y >= 0 && Y <= 7;
        }

        public int ToFieldIndex()
        {
            return (7 - X) + Y * 8;
        }

        public static Position FromFieldIndex(int fieldIndex)
        {
            return new Position(7 - fieldIndex % 8, fieldIndex / 8);
        }

        public static Position FromText(string move)
        {
            var file = move[0] - 'a';
            var rank = move[1] - '1';
            return new Position(file, rank);
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.X + b.X, a.Y + b.Y);
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position(a.X - b.X, a.Y - b.Y);
        }

        public static bool operator ==(Position a, Position b)
        {
            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Position a, Position b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public override int GetHashCode()
        {
            return X * 8 + Y;
        }

        public override string ToString()
        {
            return $"{(char)('a' + X)}{Y + 1}";
        }
    }
}