using Cosette.Engine.Board;
using Cosette.Engine.Board.Operators;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class MobilityEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase, ref ulong fieldsAttackedByWhite, ref ulong fieldsAttackedByBlack)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase, ref fieldsAttackedByWhite) - 
                   Evaluate(board, Color.Black, openingPhase, endingPhase, ref fieldsAttackedByBlack);
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase, ref ulong fieldsAttackedByColor)
        {
            var (knightCenter, knightOutside) = KnightOperator.GetMobility(board, color, ref fieldsAttackedByColor);
            var (bishopCenter, bishopOutside) = BishopOperator.GetMobility(board, color, ref fieldsAttackedByColor);
            var (rookCenter, rookOutside) = RookOperator.GetMobility(board, color, ref fieldsAttackedByColor);
            var (queenCenter, queenOutside) = QueenOperator.GetMobility(board, color, ref fieldsAttackedByColor);

            var centerMobility = knightCenter + bishopCenter + rookCenter + queenCenter;
            var centerMobilityScore = centerMobility * EvaluationConstants.CenterMobilityModifier;
            var centerMobilityScoreAdjusted = TaperedEvaluation.AdjustToPhase(centerMobilityScore, 0, openingPhase, endingPhase);

            var outsideMobility = knightOutside + bishopOutside + rookOutside + queenOutside;
            var outsideMobilityScore = outsideMobility * EvaluationConstants.OutsideMobilityModifier;
            var outsideMobilityScoreAdjusted = TaperedEvaluation.AdjustToPhase(outsideMobilityScore, 0, openingPhase, endingPhase);

            return centerMobilityScoreAdjusted + outsideMobilityScoreAdjusted;
        }
    }
}
