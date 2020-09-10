using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class MaterialEvaluator
    {
#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int Evaluate(BoardState board, float openingPhase, float endingPhase)
        {
            return board.Material[(int)Color.White] - board.Material[(int)Color.Black];
        }
    }
}
