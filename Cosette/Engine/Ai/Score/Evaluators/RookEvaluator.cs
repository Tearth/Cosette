using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class RookEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase)
        {
            var whiteEvaluation = Evaluate(board, Color.White, openingPhase, endingPhase);
            var blackEvaluation = Evaluate(board, Color.Black, openingPhase, endingPhase);
            return whiteEvaluation - blackEvaluation;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase)
        {
            var doubledRooks = 0;
            var rooksOnOpenFile = 0;
            var enemyColor = ColorOperations.Invert(color);
            var seventhRank = color == Color.White ? BoardConstants.GRank : BoardConstants.BRank;

            var rooks = board.Pieces[color][Piece.Rook];
            while (rooks != 0)
            {
                var lsb = BitOperations.GetLsb(rooks);
                var field = BitOperations.BitScan(lsb);
                rooks = BitOperations.PopLsb(rooks);

                var file = FilePatternGenerator.GetPatternForField(field) | lsb;
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

            var doubledRooksOpeningScore = doubledRooks * EvaluationConstants.DoubledRooks;
            var doubledRooksAdjusted = TaperedEvaluation.AdjustToPhase(doubledRooksOpeningScore, 0, openingPhase, endingPhase);

            var rooksOnOpenFileOpeningScore = rooksOnOpenFile * EvaluationConstants.RookOnOpenFile;
            var rooksOnOpenFileAdjusted = TaperedEvaluation.AdjustToPhase(rooksOnOpenFileOpeningScore, 0, openingPhase, endingPhase);

            return doubledRooksAdjusted + rooksOnOpenFileAdjusted;
        }
    }
}