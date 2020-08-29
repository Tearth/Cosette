using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class KnightTables
    {
        public static int[][] Pattern =
        {
            // Opening
            new[] { -10,  -5,  -5,  -5,  -5,  -5,  -5, -10,
                     -5,   0,   0,   0,   0,   0,   0,  -5,
                     -5,   0,   5,   5,   5,   5,   0,  -5,
                     -5,   5,   5,  10,  10,   5,   5,  -5,
                     -5,   0,  10,  10,  10,  10,   0,  -5,
                     -5,   5,  10,  10,  10,  10,   5,  -5,
                     -5,   5,   0,   0,   0,   0,   5,  -5,
                    -10,   0,  -5,  -5,  -5,  -5,   0, -10 },

            // Ending
            new[] { -20, -10, -10, -10, -10, -10, -10, -20,
                    -10,  0,   0,   0,   0,   0,   0,  -10,
                    -10,  0,   0,   0,   0,   0,   0,  -10,
                    -10,  0,   0,   0,   0,   0,   0,  -10,
                    -10,  0,   0,   0,   0,   0,   0,  -10,
                    -10,  0,   0,   0,   0,   0,   0,  -10,
                    -10,  0,   0,   0,   0,   0,   0,  -10,
                    -20, -10, -10, -10, -10, -10, -10, -20 }
        };

        public static int[][][] Values =
        {
            // White
            new []
            {
                TableOperations.FlipVertically(Pattern[(int)GamePhase.Opening]),
                TableOperations.FlipVertically(Pattern[(int)GamePhase.Ending])
            },

            // Black
            new []
            {
                TableOperations.FlipHorizontally(Pattern[(int)GamePhase.Opening]),
                TableOperations.FlipHorizontally(Pattern[(int)GamePhase.Ending])
            }
        };
    }
}
