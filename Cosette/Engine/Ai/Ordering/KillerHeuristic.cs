using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Ordering
{
    public static class KillerHeuristic
    {
        private static Move[][] _killerMoves;

        static KillerHeuristic()
        {
            _killerMoves = new Move[32][];

            for (var i = 0; i < _killerMoves.Length; i++)
            {
                _killerMoves[i] = new Move[MoveOrderingConstants.KillerSlots];
            }
        }

        public static void AddKillerMove(Move move, int depth)
        {
            for (var i = 0; i < MoveOrderingConstants.KillerSlots - 1; i++)
            {
                _killerMoves[depth][i + 1] = _killerMoves[depth][i];
            }

            _killerMoves[depth][0] = move;
        }

        public static bool KillerMoveExists(Move move, int depth)
        {
            for (var i = 0; i < MoveOrderingConstants.KillerSlots; i++)
            {
                if (_killerMoves[depth][i] == move)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
