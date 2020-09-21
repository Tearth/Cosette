namespace Cosette.Engine.Ai.Score
{
    public static class EvaluationConstants
    {
        public static short[] Pieces = { 100, 320, 330, 500, 1100, 20000 };

        public const short Checkmate = 32000;
        public const short ThreefoldRepetition = 0;

        public static short[] CastlingDone = { 50, 0 };
        public static short[] CastlingFailed = { -50, 0 };
        public static short[] DoubledPawns = { -20, -10 };
        public static short[] IsolatedPawns = { -20, -10 };
        public static short[] ChainedPawns = { 5, 5 };
        public static short[] PassingPawns = { 10, 50 };
        public static short[] Mobility = { 4, 1 };
        public static short[] KingInDanger = { -20, -5 };

        public const int OpeningEndgameEdge = 2 * 21000;
    }
}
