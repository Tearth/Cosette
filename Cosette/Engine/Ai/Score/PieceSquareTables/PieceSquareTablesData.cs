namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class PieceSquareTablesData
    {
        public static int[][][][] Values;

        public static void BuildPieceSquareTables()
        {
            Values = new int[6][][][]
            {
                PawnTables.Values,
                KnightTables.Build(),
                BishopTables.Build(),
                RookTables.Build(),
                QueenTables.Build(),
                KingTables.Build()
            };
        }
    }
}
