﻿using System;
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

        public static short GetHistoryMoveValue(int color, int from, int to)
        {
            return (short)(_historyMoves[color][from][to] / (_max / MoveOrderingConstants.HistoryHeuristicMaxScore));
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
