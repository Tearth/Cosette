using System;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Transposition
{
    public static class TranspositionTable
    {
        private static TranspositionTableEntry[] _table;
        private static ulong _size;

        public static unsafe void Init(ulong sizeMegabytes)
        {
            var entrySize = sizeof(TranspositionTableEntry);

            _size = sizeMegabytes * 1024ul * 1024ul / (ulong)entrySize;
            _table = new TranspositionTableEntry[_size];
        }

        public static void Add(ulong hash, byte depth, short score, byte age, Move bestMove, TranspositionTableEntryFlags flags)
        {
            _table[hash % _size] = new TranspositionTableEntry(hash, depth, score, age, bestMove, flags);
        }

        public static TranspositionTableEntry Get(ulong hash)
        {
            return _table[hash % _size];
        }

        public static void Clear()
        {
            Array.Clear(_table, 0, (int)_size);
        }
    }
}
