using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class BishopEvaluator
    {
        private const ulong KingSideWhitePawnsFianchettoPattern = 132352;
        private const ulong KingSideWhiteBishopFianchettoPattern = 512;
        private const ulong QueenSideWhitePawnsFianchettoPattern = 4235264;
        private const ulong QueenSideWhiteBishopFianchettoPattern = 16384;

        private const ulong KingSideBlackPawnsFianchettoPattern = 1409573906808832;
        private const ulong KingSideBlackBishopFianchettoPattern = 562949953421312;
        private const ulong QueenSideBlackPawnsFianchettoPattern = 45106365017882624;
        private const ulong QueenSideBlackBishopFianchettoPattern = 18014398509481984;

        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            var whiteEvaluation = Evaluate(board, Color.White, openingPhase, endingPhase);
            var blackEvaluation = Evaluate(board, Color.Black, openingPhase, endingPhase);
            return whiteEvaluation - blackEvaluation;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var kingSidePawnsPattern = color == Color.White ? KingSideWhitePawnsFianchettoPattern : KingSideBlackPawnsFianchettoPattern;
            var kingSideBishopPattern = color == Color.White ? KingSideWhiteBishopFianchettoPattern : KingSideBlackBishopFianchettoPattern;
            var queenSidePawnsPattern = color == Color.White ? QueenSideWhitePawnsFianchettoPattern : QueenSideBlackPawnsFianchettoPattern;
            var queenSideBishopPattern = color == Color.White ? QueenSideWhiteBishopFianchettoPattern : QueenSideBlackBishopFianchettoPattern;

            var pairOfBishops = 0;
            var pawns = board.Pieces[color][Piece.Pawn];
            var bishops = board.Pieces[color][Piece.Bishop];
            
            if (BitOperations.Count(bishops) > 1)
            {
                pairOfBishops = 1;
            }

            var fianchettos = 0;
            var fianchettosWithoutBishop = 0;

            if ((pawns & kingSidePawnsPattern) == kingSidePawnsPattern)
            {
                if ((bishops & kingSideBishopPattern) == kingSideBishopPattern)
                {
                    fianchettos++;
                }
                else
                {
                    fianchettosWithoutBishop++;
                }
            }

            if ((pawns & queenSidePawnsPattern) == queenSidePawnsPattern)
            {
                if ((bishops & queenSideBishopPattern) == queenSideBishopPattern)
                {
                    fianchettos++;
                }
                else
                {
                    fianchettosWithoutBishop++;
                }
            }

            var pairOfBishopsOpeningScore = pairOfBishops * EvaluationConstants.PairOfBishops;
            var fianchettosScore = fianchettos * EvaluationConstants.Fianchetto;
            var fianchettosWithoutBishopScore = fianchettosWithoutBishop * EvaluationConstants.FianchettoWithoutBishop;
            var openingScore = pairOfBishopsOpeningScore + fianchettosScore + fianchettosWithoutBishopScore;

            return TaperedEvaluation.AdjustToPhase(openingScore, 0, openingPhase, endingPhase);
        }
    }
}