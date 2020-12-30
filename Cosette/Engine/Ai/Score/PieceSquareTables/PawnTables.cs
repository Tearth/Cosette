using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class PawnTables
    {
        public static int O0 = 5;   // 7th rank
        public static int O1 = 10;  // 6th rank
        public static int O2 = 5;   // 5th rank
        public static int O3 = -15; // 4th rank with penalty
        public static int O4 = 15;  // 4th rank with reward
        public static int O5 = -5;  // 3th, 2th rank with penalty
        public static int O6 = 0;   // 3th, 2th rank with reward

        public static int E0 = 100; // 7th rank
        public static int E1 = 90;  // 6th rank
        public static int E2 = 50;  // 5th rank
        public static int E3 = 20;  // 4th rank
        public static int E4 = 0;   // 3th, 2th rank

        public static int[][][] Build()
        {
            var pattern = new[]
            {
                // Opening
                new[]
                {
                     0,  0,  0,  0,  0,  0,  0,  0,
                    O0, O0, O0, O0, O0, O0, O0, O0,
                    O1, O1, O1, O1, O1, O1, O1, O1,
                    O2, O2, O2, O2, O2, O2, O2, O2,
                    O3, O3, O3, O4, O4, O3, O3, O3,
                    O6, O6, O5, O6, O6, O5, O6, O6,
                    O6, O6, O6, O5, O5, O6, O6, O6,
                     0,  0,  0,  0,  0,  0,  0,  0
                },

                // Ending
                new[]
                {
                     0,  0,  0,  0,  0,  0,  0,  0,
                    E0, E0, E0, E0, E0, E0, E0, E0,
                    E1, E1, E1, E1, E1, E1, E1, E1,
                    E2, E2, E2, E2, E2, E2, E2, E2,
                    E3, E3, E3, E3, E3, E3, E3, E3,
                    E4, E4, E4, E4, E4, E4, E4, E4,
                    E4, E4, E4, E4, E4, E4, E4, E4,
                     0,  0,  0,  0,  0,  0,  0,  0,
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
