using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class KnightTables
    {
        public static int O0 = 10; // Center
        public static int O1 = 5;  // Center
        public static int O2 = 0;  // Center
        public static int O3 = 5;  // Original fields
        public static int O4 = 10; // Edge
        public static int O5 = 10; // Corners

        public static int E0 = 0;   // Center
        public static int E1 = 0;   // Center
        public static int E2 = 0;   // Center
        public static int E3 = -5;  // Edge
        public static int E4 = -10; // Corners

        public static int[][][] Build()
        {
            var pattern = new[]
            {
                // Opening
                new[]
                {
                    O5, O4, O4, O4, O4, O4, O4, O5,
                    O4, O2, O2, O2, O2, O2, O2, O4,
                    O4, O2, O1, O1, O1, O1, O2, O4,
                    O4, O1, O1, O0, O0, O1, O1, O4,
                    O4, O2, O0, O0, O0, O0, O2, O4,
                    O4, O1, O0, O0, O0, O0, O1, O4,
                    O4, O1, O2, O2, O2, O2, O1, O4,
                    O5, O4, O3, O4, O4, O3, O4, O5
                },

                // Ending
                new[]
                {
                    E4, E3, E3, E3, E3, E3, E3, E4,
                    E3, E2, E2, E2, E2, E2, E2, E3,
                    E3, E2, E1, E1, E1, E1, E2, E3,
                    E3, E2, E1, E0, E0, E1, E2, E3,
                    E3, E2, E1, E0, E0, E1, E2, E3,
                    E3, E2, E1, E1, E1, E1, E2, E3,
                    E3, E2, E2, E2, E2, E2, E2, E3,
                    E4, E3, E3, E3, E3, E3, E3, E4
                }
            };

            return new[]
            {
                // White
                new[]
                {
                    TableOperations.FlipVertically(pattern[GamePhase.Opening]),
                    TableOperations.FlipVertically(pattern[GamePhase.Ending])
                },

                // Black
                new[]
                {
                    TableOperations.FlipHorizontally(pattern[GamePhase.Opening]),
                    TableOperations.FlipHorizontally(pattern[GamePhase.Ending])
                }
            };
        }

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
                    -10,   0,   0,   0,   0,   0,   0, -10,
                    -10,   0,   0,   0,   0,   0,   0, -10,
                    -10,   0,   0,   0,   0,   0,   0, -10,
                    -10,   0,   0,   0,   0,   0,   0, -10,
                    -10,   0,   0,   0,   0,   0,   0, -10,
                    -10,   0,   0,   0,   0,   0,   0, -10,
                    -20, -10, -10, -10, -10, -10, -10, -20 }
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
