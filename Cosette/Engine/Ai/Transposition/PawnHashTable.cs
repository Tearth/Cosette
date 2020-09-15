using System;
using System.Runtime.CompilerServices;

namespace Cosette.Engine.Ai.Transposition
{
    public static class PawnHashTable
    {
        private static PawnHashTableEntry[] _table;
        private static ulong _size;

        public static unsafe void Init(ulong sizeMegabytes)
        {
            var entrySize = sizeof(PawnHashTableEntry);

            _size = sizeMegabytes * 1024ul * 1024ul / (ulong)entrySize;
            _table = new PawnHashTableEntry[_size];
        }

        public static void Add(ulong hash, short score)
        {
            _table[hash % _size] = new PawnHashTableEntry(hash, score);
        }

        public static PawnHashTableEntry Get(ulong hash)
        {
            return _table[hash % _size];
        }

        public static void Clear()
        {
            Array.Clear(_table, 0, (int)_size);
        }
    }
}
