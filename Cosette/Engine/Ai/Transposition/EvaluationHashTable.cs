using System;

namespace Cosette.Engine.Ai.Transposition
{
    public class EvaluationHashTable
    {
        private static EvaluationHashTableEntry[] _table;
        private static ulong _size;

        public static unsafe void Init(ulong sizeMegabytes)
        {
            var entrySize = sizeof(EvaluationHashTableEntry);

            _size = sizeMegabytes * 1024ul * 1024ul / (ulong)entrySize;
            _table = new EvaluationHashTableEntry[_size];
        }

        public static void Add(ulong hash, short score)
        {
            _table[hash % _size] = new EvaluationHashTableEntry(hash, score);
        }

        public static EvaluationHashTableEntry Get(ulong hash)
        {
            return _table[hash % _size];
        }

        public static void Clear()
        {
            Array.Clear(_table, 0, (int)_size);
        }
    }
}
