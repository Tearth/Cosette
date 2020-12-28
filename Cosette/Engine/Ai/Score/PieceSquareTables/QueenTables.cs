using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class QueenTables
    {
        public static int O0 = 10;  // Center
        public static int O1 = 5;   // Center
        public static int O2 = 0;   // Center
        public static int O3 = -5;  // Edge
        public static int O4 = -10; // Edge
        public static int O5 = -15; // Corners

        public static int E0 = 5;   // Center
        public static int E1 = 5;   // Center
        public static int E2 = 5;   // Center
        public static int E3 = 5;   // Edge
        public static int E4 = -10; // Corners

        public static int[][][] Build()
        {
            var pattern = new[]
            {
                // Opening
                new[]
                {
                    O5, O4, O4, O3, O3, O4, O4, O5,
                    O4, O2, O2, O2, O2, O2, O2, O4,
                    O4, O2, O1, O1, O1, O1, O2, O4,
                    O3, O2, O1, O0, O0, O1, O2, O3,
                    O3, O2, O1, O0, O0, O1, O2, O3,
                    O4, O2, O1, O1, O1, O1, O2, O4,
                    O4, O2, O2, O2, O2, O2, O2, O4,
                    O5, O4, O4, O3, O3, O4, O4, O5
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
    }
}
