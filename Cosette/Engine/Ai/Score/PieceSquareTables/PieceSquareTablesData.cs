namespace Cosette.Engine.Ai.Score.PieceSquareTables
{
    public static class PieceSquareTablesData
    {
        public static int[][][][] Values = new int[6][][][]
        {
            PawnTables.Values,
            KnightTables.Values,
            BishopTables.Values,
            RookTables.Values,
            QueenTables.Values,
            KingTables.Values
        };
    }
}
