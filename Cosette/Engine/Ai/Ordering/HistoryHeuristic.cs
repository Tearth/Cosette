using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Ordering
{
    public static class HistoryHeuristic
    {
        private static readonly byte[][][] _historyMoves;

        static HistoryHeuristic()
        {
            _historyMoves = new byte[2][][];
            _historyMoves[Color.White] = new byte[64][];
            _historyMoves[Color.Black] = new byte[64][];

            for (var from = 0; from < 64; from++)
            {
                _historyMoves[Color.White][from] = new byte[64];
                _historyMoves[Color.Black][from] = new byte[64];
            }
        }

        public static void AddHistoryMove(int color, int from, int to, int depth)
        {
            _historyMoves[color][from][to] = (byte)(depth * depth);
        }

        public static byte GetHistoryMoveValue(int color, int from, int to)
        {
            return _historyMoves[color][from][to];
        }

        public static void Clear()
        {
            for (var color = 0; color < 2; color++)
            {
                for (var from = 0; from < 64; from++)
                {
                    for (var to = 0; to < 64; to++)
                    {
                        _historyMoves[color][from][to] = 0;
                    }
                }
            }
        }
    }
}
