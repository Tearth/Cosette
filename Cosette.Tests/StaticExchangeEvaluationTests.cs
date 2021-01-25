using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Score.PieceSquareTables;
using Cosette.Engine.Common;
using Xunit;

namespace Cosette.Tests
{
    public class StaticExchangeEvaluationTests
    {
        public StaticExchangeEvaluationTests()
        {
            StaticExchangeEvaluation.Init();
            PieceSquareTablesData.BuildPieceSquareTables();
        }

        [Theory]
        [InlineData(Piece.Pawn, Piece.Pawn, (1 << SeePiece.Pawn), 0, 100)] // 8/8/8/3p4/4P3/8/8/8 w - - 0 1
        [InlineData(Piece.Pawn, Piece.Pawn, (1 << SeePiece.Pawn) | (1 << SeePiece.Knight1), (1 << SeePiece.Pawn) | (1 << SeePiece.Knight1), 0)] // 8/2n5/4p3/3p4/4P3/4N3/8/8 w - - 0 1
        [InlineData(Piece.Knight, Piece.Pawn, (1 << SeePiece.Pawn) | (1 << SeePiece.Knight1), (1 << SeePiece.Pawn) | (1 << SeePiece.Knight1), -250)] // 8/2n5/4p3/3p4/4P3/4N3/8/8 w - - 0 1
        [InlineData(Piece.Knight, Piece.Pawn, (1 << SeePiece.Pawn) | (1 << SeePiece.Knight1) | (1 << SeePiece.Queen), (1 << SeePiece.Pawn) | (1 << Piece.Knight), -150)] // 8/2n5/4p3/3p4/4P3/4N3/Q7/8 w - - 0 1
        [InlineData(Piece.Pawn, Piece.Pawn, (1 << SeePiece.Pawn) | (1 << SeePiece.Knight1) | (1 << SeePiece.Bishop), (1 << SeePiece.Queen), 100)] // 3q4/8/8/3p4/8/2N2B2/8/8 w - - 0 1
        [InlineData(Piece.Knight, Piece.Pawn, (1 << SeePiece.Knight1) | (1 << SeePiece.Bishop), (1 << SeePiece.Queen), 100)] // 3q4/8/8/3p4/8/2N2B2/8/8 w - - 0 1
        [InlineData(Piece.Queen, Piece.Pawn, (1 << SeePiece.Knight1) | (1 << SeePiece.Bishop) | (1 << SeePiece.Queen), (1 << SeePiece.Rook1), -520)] // 3r4/8/8/3p4/8/2N2B2/8/3Q4 w - - 0 1
        [InlineData(Piece.Knight, Piece.Pawn, (1 << SeePiece.Knight1) | (1 << SeePiece.Rook1) | (1 << SeePiece.Queen), (1 << SeePiece.Knight1) | (1 << SeePiece.Bishop) | (1 << SeePiece.Queen), -250)] // 7q/3n4/5b2/4p3/8/3N4/4R3/4Q3 w - - 0 1
        [InlineData(Piece.Rook, Piece.Pawn, (1 << SeePiece.Rook1) | (1 << SeePiece.Rook2) | (1 << SeePiece.Queen), (1 << SeePiece.Rook1) | (1 << SeePiece.Rook2), 100)] // 8/4r3/4r3/4p3/8/4R3/4R3/4Q3 w - - 0 1
        [InlineData(Piece.Rook, Piece.Pawn, (1 << SeePiece.Rook1) | (1 << SeePiece.Rook2) | (1 << SeePiece.Queen), (1 << SeePiece.Rook1) | (1 << SeePiece.Rook2) | (1 << SeePiece.Queen), -470)] // 4q3/4r3/4r3/4p3/8/4R3/4R3/4Q3 w - - 0 1
        [InlineData(Piece.Knight, Piece.Knight, (1 << SeePiece.Knight1) | (1 << SeePiece.Knight2), (1 << SeePiece.Pawn), 100)] // 8/8/2n5/5n2/3N4/2P5/8/8 w - - 0 1
        public void StaticExchangeEvaluation_NormalValues(int attackingPiece, int capturedPiece, int attacker, int defender, int expectedScore)
        {
            var score = StaticExchangeEvaluation.Evaluate(attackingPiece, capturedPiece, attacker, defender);
            Assert.Equal(expectedScore, score);
        }
    }
}