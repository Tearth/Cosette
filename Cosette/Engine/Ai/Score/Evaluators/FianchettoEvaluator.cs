using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class FianchettoEvaluator
    {
        private const ulong KingSideWhitePawnsPattern = 132352;
        private const ulong KingSideWhiteBishopPattern = 512;
        private const ulong QueenSideWhitePawnsPattern = 4235264;
        private const ulong QueenSideWhiteBishopPattern = 16384;

        private const ulong KingSideBlackPawnsPattern = 1409573906808832;
        private const ulong KingSideBlackBishopPattern = 562949953421312;
        private const ulong QueenSideBlackPawnsPattern = 45106365017882624;
        private const ulong QueenSideBlackBishopPattern = 18014398509481984;

        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            var whiteEvaluation = Evaluate(board, Color.White, openingPhase, endingPhase);
            var blackEvaluation = Evaluate(board, Color.Black, openingPhase, endingPhase);
            return whiteEvaluation - blackEvaluation;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var kingSidePawnsPattern = color == Color.White ? KingSideWhitePawnsPattern : KingSideBlackPawnsPattern;
            var kingSideBishopPattern = color == Color.White ? KingSideWhiteBishopPattern : KingSideBlackBishopPattern;
            var queenSidePawnsPattern = color == Color.White ? QueenSideWhitePawnsPattern : QueenSideBlackPawnsPattern;
            var queenSideBishopPattern = color == Color.White ? QueenSideWhiteBishopPattern : QueenSideBlackBishopPattern;

            var pawns = board.Pieces[color][Piece.Pawn];
            var bishops = board.Pieces[color][Piece.Bishop];

            if ((pawns & kingSidePawnsPattern) == kingSidePawnsPattern)
            {
                if ((bishops & kingSideBishopPattern) == kingSideBishopPattern)
                {
                    return TaperedEvaluation.AdjustToPhase(EvaluationConstants.Fianchetto, 0, openingPhase, endingPhase);
                }

                return TaperedEvaluation.AdjustToPhase(EvaluationConstants.FianchettoWithoutBishop, 0, openingPhase, endingPhase);
            }

            if ((pawns & queenSidePawnsPattern) == queenSidePawnsPattern)
            {
                if ((bishops & queenSideBishopPattern) == queenSideBishopPattern)
                {
                    return TaperedEvaluation.AdjustToPhase(EvaluationConstants.Fianchetto, 0, openingPhase, endingPhase);
                }

                return TaperedEvaluation.AdjustToPhase(EvaluationConstants.FianchettoWithoutBishop, 0, openingPhase, endingPhase);
            }

            return 0;
        }
    }
}
