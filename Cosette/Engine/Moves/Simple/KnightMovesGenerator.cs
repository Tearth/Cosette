using Cosette.Engine.Board;

namespace Cosette.Engine.Moves.Simple
{
    public static class KnightMovesGenerator
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
            return ((field & ~BoardConstants.AFile) << 17) |
                   ((field & ~BoardConstants.HFile) << 15) |
                   ((field & ~BoardConstants.AFile & ~BoardConstants.BFile) << 10) |
                   ((field & ~BoardConstants.GFile & ~BoardConstants.HFile) << 6) |
                   ((field & ~BoardConstants.AFile & ~BoardConstants.BFile) >> 6) |
                   ((field & ~BoardConstants.GFile & ~BoardConstants.HFile) >> 10) |
                   ((field & ~BoardConstants.AFile) >> 15) |
                   ((field & ~BoardConstants.HFile) >> 17);
        }
    }
}
