namespace Cosette.Engine.Ai.Score
{
    public static class EvaluationConstants
    {
        public static int[] Pieces = { 100, 300, 330, 500, 900, 20000 };

        public const int Checkmate = 65000;
        public const int ThreefoldRepetition = 0;
        public const int CastlingDone = 50;
        public const int CastlingFailed = -50;

        public const int OpeningEndgameEdge = 2 * 21000;
    }
}
