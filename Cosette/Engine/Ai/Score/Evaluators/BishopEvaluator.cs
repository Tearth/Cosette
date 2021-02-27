using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class BishopEvaluator
    {
        private const ulong WhiteKingFianchettoPattern = 7;
        private const ulong WhitePawnsFianchettoPattern = 132352;
        private const ulong WhiteBishopFianchettoPattern = 512;
        private const ulong BlackKingFianchettoPatteren = 504403158265495552;
        private const ulong BlackPawnsFianchettoPattern = 1409573906808832;
        private const ulong BlackBishopFianchettoPattern = 562949953421312;

        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            var whiteEvaluation = Evaluate(board, Color.White, openingPhase, endingPhase);
            var blackEvaluation = Evaluate(board, Color.Black, openingPhase, endingPhase);
            return whiteEvaluation - blackEvaluation;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var pairOfBishops = 0;
            if (BitOperations.Count(board.Pieces[color][Piece.Bishop]) > 1)
            {
                pairOfBishops = 1;
            }

            var fianchettos = 0;
            var fianchettosWithoutBishop = 0;
            var kingPattern = color == Color.White ? WhiteKingFianchettoPattern : BlackKingFianchettoPatteren;
            var pawnsPattern = color == Color.White ? WhitePawnsFianchettoPattern : BlackPawnsFianchettoPattern;
            var bishopPattern = color == Color.White ? WhiteBishopFianchettoPattern : BlackBishopFianchettoPattern;

            if (board.CastlingDone[color] && (board.Pieces[color][Piece.King] & kingPattern) != 0 && (board.Pieces[color][Piece.Pawn] & pawnsPattern) == pawnsPattern)
            {
                if ((board.Pieces[color][Piece.Bishop] & bishopPattern) == bishopPattern)
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