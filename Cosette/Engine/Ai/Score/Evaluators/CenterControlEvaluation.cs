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
            var piecesCount = (int)BitOperations.Count(piecesInCenter);

            var centerControlOpeningScore = piecesCount * EvaluationConstants.CenterControl;
            var centerControlEndingScore = piecesCount * EvaluationConstants.CenterControl;
            var centerControlAdjusted = TaperedEvaluation.AdjustToPhase(centerControlOpeningScore, centerControlEndingScore, openingPhase, endingPhase);

            return centerControlAdjusted;
        }
    }
}