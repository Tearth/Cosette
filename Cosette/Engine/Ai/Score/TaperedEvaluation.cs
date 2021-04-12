using System.Runtime.CompilerServices;
using Cosette.Engine.Board;

namespace Cosette.Engine.Ai.Score
{
    public static class TaperedEvaluation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AdjustToPhase(int openingScore, int endingScore, int openingPhase, int endingPhase)
        {
            return (openingScore * openingPhase + endingScore * endingPhase) / BoardConstants.PhaseResolution;
        }
    }
}
