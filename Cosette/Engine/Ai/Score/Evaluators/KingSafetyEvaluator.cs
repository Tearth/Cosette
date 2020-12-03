using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class KingSafetyEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase, ulong fieldsAttackedByWhite, ulong fieldsAttackedByBlack)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase, fieldsAttackedByBlack) - 
                   Evaluate(board, Color.Black, openingPhase, endingPhase, fieldsAttackedByWhite);
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase, ulong fieldsAttackedByEnemy)
        {
            var king = board.Pieces[color][Piece.King];
            var kingField = BitOperations.BitScan(king);
            var fieldsAroundKing = BoxPatternGenerator.GetPattern(kingField);

            var attackedFieldsAroundKing = fieldsAroundKing & fieldsAttackedByEnemy;
            var attackersCount = (int)BitOperations.Count(attackedFieldsAroundKing);

            var pawnsNearKing = fieldsAroundKing & board.Pieces[color][Piece.Pawn];
            var pawnShield = (int)BitOperations.Count(pawnsNearKing);

            var attackersCountOpeningScore = attackersCount * EvaluationConstants.KingInDanger;
            var attackersCountAdjusted = TaperedEvaluation.AdjustToPhase(attackersCountOpeningScore, 0, openingPhase, endingPhase);

            var pawnShieldOpeningScore = pawnShield * EvaluationConstants.PawnShield;
            var pawnShieldAdjusted = TaperedEvaluation.AdjustToPhase(pawnShieldOpeningScore, 0, openingPhase, endingPhase);

            return attackersCountAdjusted + pawnShieldAdjusted;
        }
    }
}
