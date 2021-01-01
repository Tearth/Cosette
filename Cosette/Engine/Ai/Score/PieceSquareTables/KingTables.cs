using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class KingTables
    {
        public static int O0 = -50; // 8th, 7yh, 6th rank
        public static int O1 = -30; // 5th, 4th rank
        public static int O2 = -30; // 3th rank
        public static int O3 = -15; // 2th, 1th rank
        public static int O4 = 0;   // King original fields
        public static int O5 = 20;  // Fields after castling

        public static int E0 = 20;  // Center
        public static int E1 = 15;  // Center
        public static int E2 = -5;  // Center
        public static int E3 = -20; // Edge

        public static int[][][] Build()
        {
            var pattern = new[]
            {
                // Opening
                new[]
                {
                    O0, O0, O0, O0, O0, O0, O0, O0,
                    O0, O0, O0, O0, O0, O0, O0, O0,
                    O0, O0, O0, O0, O0, O0, O0, O0,
                    O1, O1, O1, O1, O1, O1, O1, O1,
                    O1, O1, O1, O1, O1, O1, O1, O1,
                    O2, O2, O2, O2, O2, O2, O2, O2,
                    O3, O3, O3, O3, O3, O3, O3, O3,
                    O3, O3, O5, O4, O4, O3, O5, O3
                },

                // Ending
                new[]
                {
                    E3, E3, E3, E3, E3, E3, E3, E3,
                    E3, E2, E2, E2, E2, E2, E2, E3,
                    E3, E2, E1, E1, E1, E1, E2, E3,
                    E3, E2, E1, E0, E0, E1, E2, E3,
                    E3, E2, E1, E0, E0, E1, E2, E3,
                    E3, E2, E1, E1, E1, E1, E2, E3,
                    E3, E2, E2, E2, E2, E2, E2, E3,
                    E3, E3, E3, E3, E3, E3, E3, E3
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
