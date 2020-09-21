using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class KingSafetyEvaluator
    {
        public static int Evaluate(BoardState board, float openingPhase, float endingPhase)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase) - Evaluate(board, Color.Black, openingPhase, endingPhase);
        }

        public static int Evaluate(BoardState board, Color color, float openingPhase, float endingPhase)
        {
            var king = board.Pieces[(int)color][(int)Piece.King];
            var kingField = BitOperations.BitScan(king);
            var fieldsAroundKing = BoxPatternGenerator.GetPattern(kingField);

            var attackersCount = 0;
            while (fieldsAroundKing != 0)
            {
                var lsb = BitOperations.GetLsb(fieldsAroundKing);
                fieldsAroundKing = BitOperations.PopLsb(fieldsAroundKing);
                var field = BitOperations.BitScan(lsb);

                var attackingPieces = board.IsFieldAttacked(color, (byte)field);
                if (attackingPieces)
                {
                    attackersCount++;
                }
            }

            return (int)(attackersCount * EvaluationConstants.KingInDanger[(int)GamePhase.Opening] * openingPhase +
                         attackersCount * EvaluationConstants.KingInDanger[(int)GamePhase.Ending] * endingPhase);
        }
    }
}
