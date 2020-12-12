using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class KingSafetyEvaluator
    {
        private static ulong[][] _kingSafetyMask;

        static KingSafetyEvaluator()
        {
            _kingSafetyMask = new ulong[2][];

            for (var color = 0; color < 2; color++)
            {
                var offset = color == Color.White ? 8 : -8;
                _kingSafetyMask[color] = new ulong[64];

                for (var fieldIndex = 0; fieldIndex < 64; fieldIndex++)
                {
                    var mask = BoxPatternGenerator.GetPattern(fieldIndex);
                    var fieldIndexWithOffset = fieldIndex + offset;

                    if (fieldIndexWithOffset >= 0 && fieldIndexWithOffset < 64)
                    {
                        mask |= BoxPatternGenerator.GetPattern(fieldIndexWithOffset);
                        mask &= ~(1ul << fieldIndex);
                    }

                    _kingSafetyMask[color][fieldIndex] = mask;
                }
            }
        }

        public static int Evaluate(BoardState board, int openingPhase, int endingPhase, ulong fieldsAttackedByWhite, ulong fieldsAttackedByBlack)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase, fieldsAttackedByBlack) - 
                   Evaluate(board, Color.Black, openingPhase, endingPhase, fieldsAttackedByWhite);
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase, ulong fieldsAttackedByEnemy)
        {
            var king = board.Pieces[color][Piece.King];
            var kingField = BitOperations.BitScan(king);
            var fieldsAroundKing = _kingSafetyMask[color][kingField];

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
