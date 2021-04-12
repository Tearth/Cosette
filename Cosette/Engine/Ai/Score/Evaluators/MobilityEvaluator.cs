using Cosette.Engine.Board;
using Cosette.Engine.Board.Operators;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class MobilityEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase, ref ulong fieldsAttackedByWhite, ref ulong fieldsAttackedByBlack)
        {
            var whiteEvaluation = Evaluate(board, Color.White, openingPhase, endingPhase, ref fieldsAttackedByWhite);
            var blackEvaluation = Evaluate(board, Color.Black, openingPhase, endingPhase, ref fieldsAttackedByBlack);
            return whiteEvaluation - blackEvaluation;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase, ref ulong fieldsAttackedByColor)
        {
            var (knightCenter, knightOutside) = KnightOperator.GetMobility(board, color, ref fieldsAttackedByColor);
            var (bishopCenter, bishopOutside) = BishopOperator.GetMobility(board, color, ref fieldsAttackedByColor);
            var (rookCenter, rookOutside) = RookOperator.GetMobility(board, color, ref fieldsAttackedByColor);
            var (queenCenter, queenOutside) = QueenOperator.GetMobility(board, color, ref fieldsAttackedByColor);

            var centerMobility = knightCenter + bishopCenter + rookCenter + queenCenter;
            var centerMobilityScore = centerMobility * EvaluationConstants.CenterMobilityModifier;

            var outsideMobility = knightOutside + bishopOutside + rookOutside + queenOutside;
            var outsideMobilityScore = outsideMobility * EvaluationConstants.OutsideMobilityModifier;

            var openingScore = centerMobilityScore + outsideMobilityScore;
            return TaperedEvaluation.AdjustToPhase(openingScore, 0, openingPhase, endingPhase);
        }
    }
}
