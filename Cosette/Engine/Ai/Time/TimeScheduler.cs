using System;

namespace Cosette.Engine.Ai.Time
{
    public static class TimeScheduler
    {
        public static int CalculateTimeForMove(int remainingTime, int incTime, int moveNumber)
        {
            return remainingTime / Math.Max(20, 40 - moveNumber) + (int)(incTime * 1.5f);
        }
    }
}
