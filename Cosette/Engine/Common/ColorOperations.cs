using System.Runtime.CompilerServices;

namespace Cosette.Engine.Common
{
    public static class ColorOperations
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color Invert(Color color)
        {
            return color == Color.White ? Color.Black : Color.White;
        }
    }
}
