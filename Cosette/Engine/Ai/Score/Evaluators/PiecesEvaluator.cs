using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class PiecesEvaluator
    {
        public static int Evaluate(BoardState board, float openingPhase, float endingPhase)
        {
            return Evaluate(board, Color.White, openingPhase, endingPhase) -
                   Evaluate(board, Color.Black, openingPhase, endingPhase);
        }

        public static int Evaluate(BoardState board, int color, float openingPhase, float endingPhase)
        {
            var doubledRooks = 0;
            var rooksOnOpenFile = 0;
            var enemyColor = ColorOperations.Invert(color);

            var pieces = board.Pieces[color][Piece.Rook];
            while (pieces != 0)
            {
                var lsb = BitOperations.GetLsb(pieces);
                var field = BitOperations.BitScan(lsb);
                pieces = BitOperations.PopLsb(pieces);

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

            var doubledRooksScore = doubledRooks * EvaluationConstants.DoubledRooks[GamePhase.Opening] * openingPhase +
                                    doubledRooks * EvaluationConstants.DoubledRooks[GamePhase.Ending] * endingPhase;

            var rooksOnOpenFileScore = rooksOnOpenFile * EvaluationConstants.RookOnOpenFile[GamePhase.Opening] * openingPhase +
                                       rooksOnOpenFile * EvaluationConstants.RookOnOpenFile[GamePhase.Ending] * endingPhase;

            return (int)(doubledRooksScore + rooksOnOpenFileScore);
        }
    }
}