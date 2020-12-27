namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class TableOperations
    {
        public static int[] FlipVertically(int[] array)
        {
            var result = new int[64];
            for (var fieldIndex = 0; fieldIndex < 64; fieldIndex++)
            {
                result[fieldIndex] = array[63 - fieldIndex];
            }

            return result;
        }

        public static int[] FlipHorizontally(int[] array)
        {
            var result = new int[64];
            for (var y = 0; y < 8; y++)
            {
                for (var x = 0; x < 8; x++)
                {
                    result[x + y * 8] = array[(7 - x) + y * 8];
                }
            }

            return result;
        }
    }
}
