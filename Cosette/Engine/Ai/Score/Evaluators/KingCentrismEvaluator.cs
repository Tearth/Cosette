using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class KingCentrismEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            var whiteEvaluation = Evaluate(board, Color.White, openingPhase, endingPhase);
            var blackEvaluation = Evaluate(board, Color.Black, openingPhase, endingPhase);
            return whiteEvaluation - blackEvaluation;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var king = board.Pieces[color][Piece.King];
            var kingField = BitOperations.BitScan(king);
            var distanceToCenter = BoardConstants.DistanceFromCenter[kingField];

            var kingCentrismOpeningScore = distanceToCenter * EvaluationConstants.KingCentrism[GamePhase.Opening];
            var kingCentrismEndingScore = distanceToCenter * EvaluationConstants.KingCentrism[GamePhase.Ending];
            var kingCentrismAdjusted = TaperedEvaluation.AdjustToPhase(kingCentrismOpeningScore, kingCentrismEndingScore, openingPhase, endingPhase);

            return kingCentrismAdjusted;
        }
    }
}
