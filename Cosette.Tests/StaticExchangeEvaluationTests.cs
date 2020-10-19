using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Common;
using Xunit;

namespace Cosette.Tests
{
    public class StaticExchangeEvaluationTests
    {
        public StaticExchangeEvaluationTests()
        {
            StaticExchangeEvaluation.Init();
        }

        [Theory]
        [InlineData(Piece.Pawn, Piece.Pawn, (1 << Piece.Pawn), 0, 100)] // 8/8/8/3p4/4P3/8/8/8 w - - 0 1
        [InlineData(Piece.Pawn, Piece.Pawn, (1 << Piece.Pawn) | (1 << Piece.Knight), (1 << Piece.Pawn) | (1 << Piece.Knight), 0)] // 8/2n5/4p3/3p4/4P3/4N3/8/8 w - - 0 1
        [InlineData(Piece.Knight, Piece.Pawn, (1 << Piece.Pawn) | (1 << Piece.Knight), (1 << Piece.Pawn) | (1 << Piece.Knight), -220)] // 8/2n5/4p3/3p4/4P3/4N3/8/8 w - - 0 1
        [InlineData(Piece.Knight, Piece.Pawn, (1 << Piece.Pawn) | (1 << Piece.Knight) | (1 << Piece.Queen), (1 << Piece.Pawn) | (1 << Piece.Knight), -120)] // 8/2n5/4p3/3p4/4P3/4N3/Q7/8 w - - 0 1
        [InlineData(Piece.Pawn, Piece.Pawn, (1 << Piece.Knight) | (1 << Piece.Bishop), (1 << Piece.Queen), 100)] // 3q4/8/8/3p4/8/2N2B2/8/8 w - - 0 1
        [InlineData(Piece.Knight, Piece.Pawn, (1 << Piece.Knight) | (1 << Piece.Bishop), (1 << Piece.Queen), 100)] // 3q4/8/8/3p4/8/2N2B2/8/8 w - - 0 1
        [InlineData(Piece.Queen, Piece.Pawn, (1 << Piece.Knight) | (1 << Piece.Bishop) | (1 << Piece.Queen), (1 << Piece.Rook), -500)] // 3r4/8/8/3p4/8/2N2B2/8/3Q4 w - - 0 1
        [InlineData(Piece.Knight, Piece.Pawn, (1 << Piece.Knight) | (1 << Piece.Rook) | (1 << Piece.Queen), (1 << Piece.Knight) | (1 << Piece.Bishop) | (1 << Piece.Queen), -220)] // 7q/3n4/5b2/4p3/8/3N4/4R3/4Q3 w - - 0 1
        public void StaticExchangeEvaluation_NormalValues(int attackingPiece, int capturedPiece, int attacker, int defender, int expectedScore)
        {
            var score = StaticExchangeEvaluation.Evaluate(attackingPiece, capturedPiece, attacker, defender);
            Assert.Equal(expectedScore, score);
        }
    }
}