using System;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Transposition
{
    public static class TranspositionTable
    {
        private static TranspositionTableEntry[] _table;
        private static ulong _size;

        public static unsafe void Init(int sizeMegabytes)
        {
            var entrySize = sizeof(TranspositionTableEntry);

            _size = (ulong)sizeMegabytes * 1024ul * 1024ul / (ulong)entrySize;
            _table = new TranspositionTableEntry[_size];
        }

        public static void Add(ulong hash, TranspositionTableEntry entry)
        {
            _table[hash % _size] = entry;
        }

        public static TranspositionTableEntry Get(ulong hash)
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

        public static int RegularToTTScore(int score, int ply)
        {
            if (IterativeDeepening.IsScoreNearCheckmate(score))
            {
                if (score > 0)
                {
                    return score + ply;
                }
                else
                {
                    return score - ply;
                }
            }

            return score;
        }

        public static int TTToRegularScore(int score, int ply)
        {
            if (IterativeDeepening.IsScoreNearCheckmate(score))
            {
                if (score > 0)
                {
                    return score - ply;
                }
                else
                {
                    return score + ply;
                }
            }

            return score;
        }
    }
}
