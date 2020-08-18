using Cosette.Engine.Board;

namespace Cosette.Engine.Moves.Simple
{
    public static class KingMovesGenerator
    {
        private static ulong[] _moves;

        public static void Init()
        {
            _moves = new ulong[64];

            ulong field = 1;
            for (var i = 0; i < _moves.Length; i++)
            {
                _moves[i] = GetMoveForField(field);
                field <<= 1;
            }
        }

        public static ulong GetMoves(int fieldIndex)
        {
            return _moves[fieldIndex];
        }

        private static ulong GetMoveForField(ulong field)
        {
            return ((field & ~BoardConstants.AFile) << 1) |
                   ((field & ~BoardConstants.HFile) << 7) |
                   (field << 8) |
                   ((field & ~BoardConstants.AFile) << 9) |
                   ((field & ~BoardConstants.HFile) >> 1) |
                   ((field & ~BoardConstants.AFile) >> 7) |
                   (field >> 8) |
                   ((field & ~BoardConstants.HFile) >> 9);
        }
    }
}
