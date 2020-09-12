using System.Runtime.CompilerServices;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class PositionEvaluator
    {
#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int Evaluate(BoardState board, float openingPhase, float endingPhase)
        {
            var whitePositionScore = board.Position[(int)Color.White][(int)GamePhase.Opening] * openingPhase +
                                     board.Position[(int)Color.White][(int)GamePhase.Ending] * endingPhase;

            var blackPositionScore = board.Position[(int)Color.Black][(int)GamePhase.Opening] * openingPhase +
                                     board.Position[(int)Color.Black][(int)GamePhase.Ending] * endingPhase;

            return (int)whitePositionScore - (int)blackPositionScore;
        }
    }
}
