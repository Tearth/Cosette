using System;

namespace Cosette.Engine.Ai.Time
{
    public static class TimeScheduler
    {
        public static int CalculateTimeForMove(int remainingTime, int moveNumber)
        {
            return remainingTime / Math.Max(20, 40 - moveNumber);
        }
    }
}
