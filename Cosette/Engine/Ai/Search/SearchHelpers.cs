namespace Cosette.Engine.Ai.Search
{
    public static class SearchHelpers
    {
        public static int PostProcessScore(int score)
        {
            if (IterativeDeepening.IsScoreNearCheckmate(score))
            {
                if (score > 0)
                {
                    return score - 1;
                }
                else
                {
                    return score + 1;
                }
            }

            return score;
        }
    }
}
