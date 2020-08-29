using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class QueenTables
    {
        public static int[][] Pattern =
        {
            // Opening
            new[] { -15, -10, -10,  -5,  -5, -10, -10,  -15,
                    -10,   0,   0,   0,   0,   0,   0,  -10,
                    -10,   0,   5,   5,   5,   5,   0,  -10,
                     -5,   0,   5,  10,  10,   5,   0,   -5,
                     -5,   0,   5,  10,  10,   5,   0,   -5,
                    -10,   0,   5,   5,   5,   5,   0,  -10,
                    -10,   0,   0,   0,   0,   0,   0,  -10,
                    -15, -10, -10,  -5,  -5,  -10, -10, -15 },

            // Ending
            new[] { 5,   5,   5,   5,   5,   5,   5,   5,
                    5,   5,   5,   5,   5,   5,   5,   5,
                    5,   5,   5,   5,   5,   5,   5,   5,
                    5,   5,   5,   5,   5,   5,   5,   5,
                    5,   5,   5,   5,   5,   5,   5,   5,
                    5,   5,   5,   5,   5,   5,   5,   5,
                    5,   5,   5,   5,   5,   5,   5,   5,
                    5,   5,   5,   5,   5,   5,   5,   5}
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
