using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class PositionEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            var whiteEvaluation = Evaluate(board, Color.White, openingPhase, endingPhase);
            var blackEvaluation = Evaluate(board, Color.Black, openingPhase, endingPhase);
            return whiteEvaluation - blackEvaluation;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var sensitivePieces = board.Pieces[color][Piece.Knight] | board.Pieces[color][Piece.Bishop] | board.Pieces[color][Piece.Queen];
            var piecesOnEdge = BoardConstants.Edges & sensitivePieces;
            var piecesCount = (int)BitOperations.Count(piecesOnEdge);

            var piecesOnEdgeOpeningScore = piecesCount * EvaluationConstants.PieceOnEdge;
            var piecesOnEdgeAdjusted = TaperedEvaluation.AdjustToPhase(piecesOnEdgeOpeningScore, 0, openingPhase, endingPhase);

            return piecesOnEdgeAdjusted;
        }
    }
}