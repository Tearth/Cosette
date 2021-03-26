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
            _historyMoves[Color.White] = new uint[6][];
            _historyMoves[Color.Black] = new uint[6][];

            for (var piece = 0; piece < 6; piece++)
            {
                _historyMoves[Color.White][piece] = new uint[64];
                _historyMoves[Color.Black][piece] = new uint[64];
            }

            _max = MoveOrderingConstants.HistoryHeuristicMaxScore;
        }

        public static void AddHistoryMove(int color, int piece, int to, int depth)
        {
            var newValue = _historyMoves[color][piece][to] + (uint)(depth * depth);

            _max = Math.Max(_max, newValue);
            _historyMoves[color][piece][to] = newValue;
        }

        public static short GetMoveValue(int color, int piece, int to)
        {
            return (short)Math.Ceiling(_historyMoves[color][piece][to] / ((float)_max / MoveOrderingConstants.HistoryHeuristicMaxScore));
        }

        public static uint GetRawMoveValue(int color, int piece, int to)
        {
            return _historyMoves[color][piece][to];
        }

        public static uint GetMaxValue()
        {
            return _max;
        }

        public static void AgeValues()
        {
            for (var color = 0; color < 2; color++)
            {
                for (var piece = 0; piece < 6; piece++)
                {
                    for (var to = 0; to < 64; to++)
                    {
                        _historyMoves[color][piece][to] /= 2;
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
                for (var piece = 0; piece < 6; piece++)
                {
                    Array.Clear(_historyMoves[color][piece], 0, 64);
                }
            }

            _max = MoveOrderingConstants.HistoryHeuristicMaxScore;
        }
    }
}
