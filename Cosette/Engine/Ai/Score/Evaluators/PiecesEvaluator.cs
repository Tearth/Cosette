using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class PiecesEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase) -
                   Evaluate(board, Color.Black, openingPhase, endingPhase);
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var doubledRooks = 0;
            var rooksOnOpenFile = 0;
            var pairOfBishops = 0;
            var enemyColor = ColorOperations.Invert(color);

            var rooks = board.Pieces[color][Piece.Rook];
            while (rooks != 0)
            {
                var lsb = BitOperations.GetLsb(rooks);
                var field = BitOperations.BitScan(lsb);
                rooks = BitOperations.PopLsb(rooks);

                var file = FilePatternGenerator.GetPattern(field) | lsb;
                var rooksOnFile = file & board.Pieces[color][Piece.Rook];
                var friendlyPawnsOnFile = file & board.Pieces[color][Piece.Pawn];
                var enemyPawnsOnFile = file & board.Pieces[enemyColor][Piece.Pawn];

                if (BitOperations.Count(rooksOnFile) > 1)
                {
                    // We don't assume that there will be more than two rooks - even if, then this color is probably anyway winning
                    doubledRooks = 1;
                }

                if (friendlyPawnsOnFile == 0 && enemyPawnsOnFile == 0)
                {
                    rooksOnOpenFile++;
                }
            }

            var bishops = board.Pieces[color][Piece.Bishop];
            if (BitOperations.Count(bishops) > 1)
            {
                pairOfBishops = 1;
            }

            var doubledRooksOpeningScore = doubledRooks * EvaluationConstants.DoubledRooks[GamePhase.Opening];
            var doubledRooksEndingScore = doubledRooks * EvaluationConstants.DoubledRooks[GamePhase.Ending];
            var doubledRooksAdjusted = TaperedEvaluation.AdjustToPhase(doubledRooksOpeningScore, doubledRooksEndingScore, openingPhase, endingPhase);

            var rooksOnOpenFileOpeningScore = rooksOnOpenFile * EvaluationConstants.RookOnOpenFile[GamePhase.Opening];
            var rooksOnOpenFileEndingScore = rooksOnOpenFile * EvaluationConstants.RookOnOpenFile[GamePhase.Ending];
            var rooksOnOpenFileAdjusted = TaperedEvaluation.AdjustToPhase(rooksOnOpenFileOpeningScore, rooksOnOpenFileEndingScore, openingPhase, endingPhase);

            var pairOfBishopsOpeningScore = pairOfBishops * EvaluationConstants.PairOfBishops[GamePhase.Opening];
            var pairOfBishopsEndingScore = pairOfBishops * EvaluationConstants.PairOfBishops[GamePhase.Ending];
            var pairOfBishopsAdjusted = TaperedEvaluation.AdjustToPhase(pairOfBishopsOpeningScore, pairOfBishopsEndingScore, openingPhase, endingPhase);

            return doubledRooksAdjusted + rooksOnOpenFileAdjusted + pairOfBishopsAdjusted;
        }
    }
}