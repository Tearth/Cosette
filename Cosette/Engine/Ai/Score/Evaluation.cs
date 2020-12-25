using Cosette.Engine.Ai.Score.Evaluators;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Score
{
    public class Evaluation
    {
        public static int Evaluate(BoardState board, bool enableCache, EvaluationStatistics statistics)
        {
            var openingPhase = board.GetPhaseRatio();
            var endingPhase = BoardConstants.PhaseResolution - openingPhase;

            var result = MaterialEvaluator.Evaluate(board);
            result += enableCache ? 
                PawnStructureEvaluator.Evaluate(board, statistics, openingPhase, endingPhase) :
                PawnStructureEvaluator.EvaluateWithoutCache(board, statistics, openingPhase, endingPhase);
            result += KingCentrismEvaluator.Evaluate(board, openingPhase, endingPhase);

            if (endingPhase != BoardConstants.PhaseResolution)
            {
                var fieldsAttackedByWhite = 0ul;
                var fieldsAttackedByBlack = 0ul;

                result += MobilityEvaluator.Evaluate(board, openingPhase, endingPhase, ref fieldsAttackedByWhite, ref fieldsAttackedByBlack);
                result += KingSafetyEvaluator.Evaluate(board, openingPhase, endingPhase, fieldsAttackedByWhite, fieldsAttackedByBlack);
                result += CastlingEvaluator.Evaluate(board, openingPhase, endingPhase);
                result += FianchettoEvaluator.Evaluate(board, openingPhase, endingPhase);
                result += PiecesEvaluator.Evaluate(board, openingPhase, endingPhase);
                result += CenterControlEvaluator.Evaluate(board, openingPhase, endingPhase);
                result += PositionEvaluator.Evaluate(board, openingPhase, endingPhase);
            }

            return board.ColorToMove == Color.White ? result : -result;
        }

        public static int FastEvaluate(BoardState board)
        {
            var result = MaterialEvaluator.Evaluate(board);
            return board.ColorToMove == Color.White ? result : -result;
        }
    }
}
