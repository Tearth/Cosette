namespace Cosette.Engine.Common
{
    public readonly struct Position
    {
        public int X { get; }
        public int Y { get; }

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
            return X + Y * 8;
        }

        public static Position FromFieldIndex(int fieldIndex)
        {
            return new Position(fieldIndex % 8, fieldIndex / 8);
        }

        public static Position operator +(Position a, Position b)
        {
            return new Position(a.X + b.X, a.Y + b.Y);
        }

        public static Position operator -(Position a, Position b)
        {
            return new Position(a.X - b.X, a.Y - b.Y);
        }

        public override int GetHashCode()
        {
            return X * 8 + Y;
        }

        public override string ToString()
        {
            return $"[{X} {Y}]";
        }
    }
}