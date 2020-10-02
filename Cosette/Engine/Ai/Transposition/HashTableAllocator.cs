using System;

namespace Cosette.Engine.Ai.Transposition
{
    public static class HashTableAllocator
    {
        public static void Allocate()
        {
            Allocate(HashTableConstants.DefaultHashTablesSize);
        }

        public static void Allocate(int megabytes)
        {
            var pawnHashTableSize = Math.Max(1, megabytes / HashTableConstants.PawnHashTableSizeDivider);
            var evaluationHashTableSize = Math.Max(1, megabytes / HashTableConstants.EvaluationHashTableSizeDivider);
            var transpositionTableSize = megabytes - pawnHashTableSize - evaluationHashTableSize;

            Allocate(transpositionTableSize, pawnHashTableSize, evaluationHashTableSize);
        }

        public static void Allocate(int transpositionTableMegabytes, int evaluationHashTableMegabytes, int pawnHashTableMegabytes)
        {
            TranspositionTable.Init(transpositionTableMegabytes);
            PawnHashTable.Init(evaluationHashTableMegabytes);
            EvaluationHashTable.Init(pawnHashTableMegabytes);
        }
    }
}
