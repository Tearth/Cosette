using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Ordering
{
    public static class HistoryHeuristic
    {
        private static readonly byte[][][] _historyMoves;

        static HistoryHeuristic()
        {
            _historyMoves = new byte[2][][];
            _historyMoves[(int)Color.White] = new byte[64][];
            _historyMoves[(int)Color.Black] = new byte[64][];

            for (var i = 0; i < 64; i++)
            {
                _historyMoves[(int)Color.White][i] = new byte[64];
                _historyMoves[(int)Color.Black][i] = new byte[64];
            }
        }

        public static void AddHistoryMove(Color color, int from, int to, int depth)
        {
            _historyMoves[(int)color][from][to] = (byte)(depth * depth);
        }

        public static byte GetHistoryMoveValue(Color color, int from, int to)
        {
            return _historyMoves[(int)color][from][to];
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
