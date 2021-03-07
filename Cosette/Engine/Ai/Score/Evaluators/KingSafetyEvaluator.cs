using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves.Patterns;
using System;

namespace Cosette.Engine.Ai.Score.Evaluators
{
    public static class KingSafetyEvaluator
    {
        public static int Evaluate(BoardState board, int openingPhase, int endingPhase, ulong fieldsAttackedByWhite, ulong fieldsAttackedByBlack)
        {
            var whiteEvaluation = Evaluate(board, Color.White, openingPhase, endingPhase, fieldsAttackedByBlack);
            var blackEvaluation = Evaluate(board, Color.Black, openingPhase, endingPhase, fieldsAttackedByWhite);
            return whiteEvaluation - blackEvaluation;
        }

        public static int Evaluate(BoardState board, int color, int openingPhase, int endingPhase, ulong fieldsAttackedByEnemy)
        {
            var enemyColor = ColorOperations.Invert(color);

            var king = board.Pieces[color][Piece.King];
            var kingField = BitOperations.BitScan(king);
            var kingPosition = Position.FromFieldIndex(kingField);
            var fieldsAroundKing = ForwardBoxPatternGenerator.GetPattern(color, kingField);

            var attackedFieldsAroundKing = fieldsAroundKing & fieldsAttackedByEnemy;
            var attackersCount = (int)BitOperations.Count(attackedFieldsAroundKing);
            var attackersCountOpeningScore = attackersCount * EvaluationConstants.KingInDanger;

            var pawnShieldOpeningScore = 0;
            if (board.CastlingDone[board.ColorToMove])
            {
                var pawnsNearKing = fieldsAroundKing & board.Pieces[color][Piece.Pawn];
                var pawnShield = (int)BitOperations.Count(pawnsNearKing);

                pawnShieldOpeningScore = pawnShield * EvaluationConstants.PawnShield;
            }

            var tropismOpeningScore = 0;
            for (var pieceType = Piece.Pawn; pieceType <= Piece.Queen; pieceType++)
            {
                var pieces = board.Pieces[enemyColor][pieceType];
                while (pieces != 0)
                {
                    var field = BitOperations.GetLsb(pieces);
                    var fieldIndex = BitOperations.BitScan(field);
                    pieces = BitOperations.PopLsb(pieces);

                    tropismOpeningScore += Distance.Table[kingField][fieldIndex] * EvaluationConstants.Tropism[pieceType];
                }
            }

            var openFileCheckFrom = Math.Max(0, kingPosition.X - 1);
            var openFileCheckTo = Math.Min(7, kingPosition.X + 1);
            var openFilesNextToKingScore = 0;

            for (var file = openFileCheckFrom; file < openFileCheckTo; file++)
            {
                if ((FilePatternGenerator.GetPatternForFile(file) & board.Pieces[color][Piece.Pawn]) == 0)
                {
                    openFilesNextToKingScore += EvaluationConstants.OpenFileNextToKing;
                }
            }

            var openingScore = pawnShieldOpeningScore + attackersCountOpeningScore + tropismOpeningScore + openFilesNextToKingScore;
            return TaperedEvaluation.AdjustToPhase(openingScore, 0, openingPhase, endingPhase);
        }
    }
}
