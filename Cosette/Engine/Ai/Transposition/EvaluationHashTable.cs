using System;

namespace Cosette.Engine.Ai.Transposition
{
    public class EvaluationHashTable
    {
        private static EvaluationHashTableEntry[] _table;
        private static ulong _size;

        public static unsafe void Init(int sizeMegabytes)
        {
            var entrySize = sizeof(EvaluationHashTableEntry);

            _size = (ulong)sizeMegabytes * 1024ul * 1024ul / (ulong)entrySize;
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

        public static float GetFillLevel()
        {
            var filledEntries = 0;
            for (var i = 0; i < 1000; i++)
            {
                if (_table[i].Key != 0 || _table[i].Score != 0)
                {
                    filledEntries++;
                }
            }

            return (float)filledEntries / 10;
        }

        public static void Clear()
        {
            Array.Clear(_table, 0, (int)_size);
        }
    }
}
