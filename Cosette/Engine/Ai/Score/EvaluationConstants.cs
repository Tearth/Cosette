namespace Cosette.Engine.Ai.Score
{
    public static class EvaluationConstants
    {
        public static int[][] Pieces =
        {
            new [] { 100, 300, 300, 500, 1000, 10000 },
            new [] { -100, -300, -300, -500, -1000, -10000 }
        };

        public static int CastlingDone = 50;
        public static int CastlingFailed = -50;
    }
}
