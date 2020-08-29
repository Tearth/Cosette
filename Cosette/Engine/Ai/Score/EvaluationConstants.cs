namespace Cosette.Engine.Ai.Score
{
    public static class EvaluationConstants
    {
        public static int[][] Pieces =
        {
            new [] { 100, 300, 330, 500, 900, 20000 },
            new [] { -100, -300, -330, -500, -900, -20000 }
        };

        public const int CastlingDone = 50;
        public const int CastlingFailed = -50;
    }
}
