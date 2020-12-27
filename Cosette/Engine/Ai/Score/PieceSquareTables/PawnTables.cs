﻿using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class PawnTables
    {
        public static int[][] Pattern =
        {
            // Opening
            new[] {  20,  20,  20,  20,  20,  20,  20,  20,
                     15,  15,  15,  15,  15,  15,  15,  15,
                     10,  10,  10,  10,  10,  10,  10,  10,
                      5,   5,   5,   5,   5,   5,   5,   5,
                    -15, -15, -15,  10,  10, -15, -15, -15,
                     10,  10, -10,   5,   5, -10,  10,   5,
                     10,  10,  10, -10, -10,  10,  10,  10,
                      0,   0,   0,   0,   0,   0,   0,   0 },

            // Ending
            new[] {  90,  90,  90,  90,  90,  90,  90,  90,
                     70,  70,  70,  70,  70,  70,  70,  70,
                     50,  50,  50,  50,  50,  50,  50,  50,
                     30,  30,  30,  30,  30,  30,  30,  30,
                     10,  10,  10,  10,  10,  10,  10,  10,
                      0,   0,   0,   0,   0,   0,   0,   0,
                      0,   0,   0,   0,   0,   0,   0,   0,
                      0,   0,   0,   0,   0,   0,   0,   0 }
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
