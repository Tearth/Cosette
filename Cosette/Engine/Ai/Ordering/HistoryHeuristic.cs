using System;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Ordering
{
    public static class HistoryHeuristic
    {
        private static readonly uint[][][] _historyMoves;
        private static uint _max;

        static HistoryHeuristic()
        {
            _historyMoves = new uint[2][][];
            _historyMoves[Color.White] = new uint[64][];
            _historyMoves[Color.Black] = new uint[64][];

            for (var from = 0; from < 64; from++)
            {
                _historyMoves[Color.White][from] = new uint[64];
                _historyMoves[Color.Black][from] = new uint[64];
            }

            _max = MoveOrderingConstants.HistoryHeuristicMaxScore;
        }

        public static void AddHistoryMove(int color, int from, int to, int depth)
        {
            var newValue = _historyMoves[color][from][to] + (uint)(depth * depth);

            _max = Math.Max(_max, newValue);
            _historyMoves[color][from][to] = newValue;
        }

        public static short GetMoveValue(int color, int from, int to)
        {
            return (short)Math.Ceiling(_historyMoves[color][from][to] / ((float)_max / MoveOrderingConstants.HistoryHeuristicMaxScore));
        }

        public static uint GetRawMoveValue(int color, int from, int to)
        {
            return _historyMoves[color][from][to];
        }

        public static uint GetMaxValue()
        {
            return _max;
        }

        public static void AgeValues()
        {
            for (var color = 0; color < 2; color++)
            {
                for (var from = 0; from < 64; from++)
                {
                    for (var to = 0; to < 64; to++)
                    {
                        _historyMoves[color][from][to] /= 2;
                    }
                }
            }
            
            _max /= 2;
            _max = Math.Max(_max, MoveOrderingConstants.HistoryHeuristicMaxScore);
        }

        public static void Clear()
        {
            for (var color = 0; color < 2; color++)
            {
                for (var from = 0; from < 64; from++)
                {
                    Array.Clear(_historyMoves[color][from], 0, 64);
                }
            }

            _max = MoveOrderingConstants.HistoryHeuristicMaxScore;
        }
    }
}
