using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class PositionEvaluator
    {
        public static int Evaluate(BoardState board, float openingPhase, float endingPhase)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase) -
                   Evaluate(board, Color.Black, openingPhase, endingPhase);
        }

        public static int Evaluate(BoardState board, int color, float openingPhase, float endingPhase)
        {
            return (int)(board.Position[color][GamePhase.Opening] * openingPhase +
                         board.Position[color][GamePhase.Ending] * endingPhase);
        }
    }
}
