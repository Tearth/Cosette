using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class CenterControlEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            var whiteEvaluation = Evaluate(board, Color.White, openingPhase, endingPhase);
            var blackEvaluation = Evaluate(board, Color.Black, openingPhase, endingPhase);
            return whiteEvaluation - blackEvaluation;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var availablePieces = board.Pieces[color][Piece.Pawn] | board.Pieces[color][Piece.Knight] |
                                  board.Pieces[color][Piece.Bishop] | board.Pieces[color][Piece.Queen];

            var piecesInCenter = EvaluationConstants.Center & availablePieces;
            var piecesInCenterCount = (int)BitOperations.Count(piecesInCenter);

            var piecesInExtendedCenterRing = EvaluationConstants.ExtendedCenterRing & availablePieces;
            var piecesInExtendedCenterRingCount = (int)BitOperations.Count(piecesInExtendedCenterRing);

            var centerControlOpeningScore = piecesInCenterCount * EvaluationConstants.CenterControl;
            var centerControlAdjusted = TaperedEvaluation.AdjustToPhase(centerControlOpeningScore, 0, openingPhase, endingPhase);

            var extendedCenterControlOpeningScore = piecesInExtendedCenterRingCount * EvaluationConstants.ExtendedCenterControl;
            var extendedCenterControlAdjusted = TaperedEvaluation.AdjustToPhase(extendedCenterControlOpeningScore, 0, openingPhase, endingPhase);

            return centerControlAdjusted + extendedCenterControlAdjusted;
        }
    }
}