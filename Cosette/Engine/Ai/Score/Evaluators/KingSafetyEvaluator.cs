using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class KingSafetyEvaluator
    {
        public static int Evaluate(BoardState board, float openingPhase, float endingPhase)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase) - 
                   Evaluate(board, Color.Black, openingPhase, endingPhase);
        }

        public static int Evaluate(BoardState board, int color, float openingPhase, float endingPhase)
        {
            var king = board.Pieces[color][Piece.King];
            var kingField = BitOperations.BitScan(king);
            var fieldsAroundKing = BoxPatternGenerator.GetPattern(kingField);

            var attackersCount = 0;
            while (fieldsAroundKing != 0)
            {
                var lsb = BitOperations.GetLsb(fieldsAroundKing);
                var field = BitOperations.BitScan(lsb);
                fieldsAroundKing = BitOperations.PopLsb(fieldsAroundKing);

                var attackingPieces = board.IsFieldAttacked(color, (byte)field);
                if (attackingPieces)
                {
                    attackersCount++;
                }
            }

            return (int)(attackersCount * EvaluationConstants.KingInDanger[GamePhase.Opening] * openingPhase +
                         attackersCount * EvaluationConstants.KingInDanger[GamePhase.Ending] * endingPhase);
        }
    }
}
