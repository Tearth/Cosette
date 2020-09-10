namespace Cosette.Engine.Ai.Score
{
    public static class EvaluationConstants
    {
        public static int[] Pieces = { 100, 320, 330, 500, 1100, 20000 };

        public const int Checkmate = 32000;
        public const int ThreefoldRepetition = 0;

        public static int[] CastlingDone = { 50, 0 };
        public static int[] CastlingFailed = { -50, 0 };
        public static int[] DoubledPawns = { -20, -10 };
        public static int[] IsolatedPawns = { -20, -10 };
        public static int[] ChainedPawns = { 5, 5 };
        public static int[] PassingPawns = { 20, 100 };
        public static int[] Mobility = { 4, 1 };
        public static int[] KingSafety = { -20, -5 };

        public const int OpeningEndgameEdge = 2 * 21000;
    }
}
