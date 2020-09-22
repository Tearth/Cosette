using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class KingTables
    {
        public static int[][] Pattern =
        {
            // Opening
            new[] { -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -30, -40, -40, -50, -50, -40, -40, -30,
                    -20, -30, -30, -40, -40, -30, -30, -20,
                    -10, -20, -20, -20, -20, -20, -20, -10,
                    -5,   -5, -10,  -5,  -5,  -5, -10,  -5,
                     0,    5,  20, -10,   0, -10,  20,   0 },

            // Ending
            new[] { -10, -10, -10, -10, -10, -10, -10, -10,
                    -10,   0,   0,   0,   0,   0,   0, -10,
                    -10,   0,  10,  10,  10,  10,   0, -10,
                    -10,   0,  10,  15,  15,  10,   0, -10,
                    -10,   0,  10,  15,  15,  10,   0, -10,
                    -10,   0,  10,  10,  10,  10,   0, -10,
                    -10,   0,   0,   0,   0,   0,   0, -10,
                    -10, -10, -10, -10, -10, -10, -10, -10 }
        };

        public static int[][][] Values =
        {
            // White
            new []
            {
                TableOperations.FlipVertically(Pattern[GamePhase.Opening]),
                TableOperations.FlipVertically(Pattern[GamePhase.Ending])
            },

            // Black
            new []
            {
                TableOperations.FlipHorizontally(Pattern[GamePhase.Opening]),
                TableOperations.FlipHorizontally(Pattern[GamePhase.Ending])
            }
        };
    }
}
