namespace Cosette.Engine.Ai.Transposition
{
    public static class HashTableConstants
    {
        public const int DefaultHashTablesSize = 32;

        /*
         * For every 128 megabytes of memory, there are:
         *  - 105 megabytes of transposition table
         *  - 21 megabytes of evaluation hash table
         *  - 2 megabyte of pawn hash table
         */
        public const int PawnHashTableSizeDivider = 64;
        public const int EvaluationHashTableSizeDivider = 6;
    }
}
