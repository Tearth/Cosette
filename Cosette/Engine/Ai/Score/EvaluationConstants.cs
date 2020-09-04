namespace Cosette.Engine.Ai.Score
{
    public static class EvaluationConstants
    {
        public static int[] Pieces = { 100, 320, 330, 500, 1100, 20000 };

        public const int Checkmate = 32000;
        public const int ThreefoldRepetition = 0;
        public const int CastlingDone = 50;
        public const int CastlingFailed = -50;
        public const int DoubledPawns = -5;
        public const int IsolatedPawns = -5;
        public const int ChainedPawns = 5;
        public const int Mobility = 2;

        public const int OpeningEndgameEdge = 2 * 21000;
    }
}
