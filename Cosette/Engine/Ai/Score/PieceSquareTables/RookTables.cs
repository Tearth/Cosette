using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class RookTables
    {
        public static int O0 = 0;  // Center
        public static int O1 = 5;  // First rank
        public static int O2 = 5;  // Edge fields near to rook
        public static int O3 = 10; // Edge fields near to enemy
        public static int O4 = 10; // Fields after castling

        public static int E0 = 5;  // Center
        public static int E1 = 5;  // Center
        public static int E2 = 5;  // Center
        public static int E3 = 5;  // Edge

        public static int[][][] Build()
        {
            var pattern = new[]
            {
                // Opening
                new[]
                {  
                    O3, O0, O0, O0, O0, O0, O0, O3,
                    O3, O0, O0, O0, O0, O0, O0, O3,
                    O3, O0, O0, O0, O0, O0, O0, O3,
                    O3, O0, O0, O0, O0, O0, O0, O3,
                    O2, O0, O0, O0, O0, O0, O0, O2,
                    O2, O0, O0, O0, O0, O0, O0, O2,
                    O2, O0, O0, O0, O0, O0, O0, O2,
                    O1, O1, O1, O4, O1, O4, O1, O1
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
