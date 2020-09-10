using Cosette.Engine.Ai.Search;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Ordering
{
    public static class KillerHeuristic
    {
        private static Move[][][] _killerMoves;

        static KillerHeuristic()
        {
            _killerMoves = new Move[2][][];
            _killerMoves[(int) Color.White] = new Move[SearchConstants.MaxDepth][];
            _killerMoves[(int) Color.Black] = new Move[SearchConstants.MaxDepth][];

            for (var i = 0; i < SearchConstants.MaxDepth; i++)
            {
                _killerMoves[(int)Color.White][i] = new Move[MoveOrderingConstants.KillerSlots];
                _killerMoves[(int)Color.Black][i] = new Move[MoveOrderingConstants.KillerSlots];
            }
        }

        public static void AddKillerMove(Move move, Color color, int depth)
        {
            for (var i = 0; i < MoveOrderingConstants.KillerSlots - 1; i++)
            {
                _killerMoves[(int) color][depth][i + 1] = _killerMoves[(int)color][depth][i];
            }

            _killerMoves[(int)color][depth][0] = move;
        }

        public static bool KillerMoveExists(Move move, Color color, int depth)
        {
            for (var i = 0; i < MoveOrderingConstants.KillerSlots; i++)
            {
                if (_killerMoves[(int)color][depth][i] == move)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
