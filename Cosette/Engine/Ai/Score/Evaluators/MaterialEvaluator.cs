using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class MaterialEvaluator
    {
        public static int Evaluate(BoardState board, float openingPhase, float endingPhase)
        {
            return board.Material[Color.White] - board.Material[Color.Black];
        }
    }
}
