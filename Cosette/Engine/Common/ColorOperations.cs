namespace Cosette.Engine.Common
{
    public static class ColorOperations
    {
        public static int Invert(int color)
        {
            return color == Color.White ? Color.Black : Color.White;
        }
    }
}
