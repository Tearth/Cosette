using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Ordering
{
    public static class HistoryHeuristic
    {
        private static readonly int[][][] _historyMoves;

        static HistoryHeuristic()
        {
            _historyMoves = new int[2][][];
            _historyMoves[(int)Color.White] = new int[64][];
            _historyMoves[(int)Color.Black] = new int[64][];

            for (var i = 0; i < 64; i++)
            {
                _historyMoves[(int)Color.White][i] = new int[64];
                _historyMoves[(int)Color.Black][i] = new int[64];
            }
        }

        public static void AddHistoryMove(Color color, byte from, byte to, int value)
        {
            _historyMoves[(int)color][from][to] = value;
        }

        public static int GetHistoryMoveValue(Color color, byte from, byte to)
        {
            return _historyMoves[(int) color][from][to];
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
