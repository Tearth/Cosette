using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai
{
    public static class TranspositionTable
    {
        private static Dictionary<ulong, TranspositionTableEntry> _table;

        public static void Init()
        {
            _table = new Dictionary<ulong, TranspositionTableEntry>(100_000_000);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Add(ulong hash, int depth, int score, Move bestMove, TranspositionTableEntryType type)
        {
            _table[hash] = new TranspositionTableEntry(depth, score, bestMove, type);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TranspositionTableEntry Get(ulong hash)
        {
            return _table[hash];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Exists(ulong hash)
        {
            return _table.ContainsKey(hash);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Clear()
        {
            _table.Clear();
        }
    }
}
