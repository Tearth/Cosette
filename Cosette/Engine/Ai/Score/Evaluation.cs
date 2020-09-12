using System.Runtime.CompilerServices;
using Cosette.Engine.Ai.Score.Evaluators;
using Cosette.Engine.Ai.Score.PieceSquareTables;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Board;
using Cosette.Engine.Board.Operators;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score
{
    public class Evaluation
    {
#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static int Evaluate(BoardState board, Color color)
        {
            var openingPhase = board.GetPhaseRatio();
            var endingPhase = 1 - openingPhase;

            var result = MaterialEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += CastlingEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += PositionEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += PawnStructureEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += MobilityEvaluator.Evaluate(board, openingPhase, endingPhase);
            result += KingSafetyEvaluator.Evaluate(board, openingPhase, endingPhase);

            var sign = color == Color.White ? 1 : -1;
            return sign * result;
        }
    }
}
