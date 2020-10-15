using System;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Ordering
{
    public static class KillerHeuristic
    {
        private static readonly Move[][][] _killerMoves;

        static KillerHeuristic()
        {
            _killerMoves = new Move[2][][];
            _killerMoves[Color.White] = new Move[SearchConstants.MaxDepth][];
            _killerMoves[Color.Black] = new Move[SearchConstants.MaxDepth][];

            for (var depth = 0; depth < SearchConstants.MaxDepth; depth++)
            {
                _killerMoves[Color.White][depth] = new Move[MoveOrderingConstants.KillerSlots];
                _killerMoves[Color.Black][depth] = new Move[MoveOrderingConstants.KillerSlots];
            }
        }

        public static void AddKillerMove(Move move, int color, int depth)
        {
            for (var slot = MoveOrderingConstants.KillerSlots - 2; slot >= 0; slot--)
            {
                _killerMoves[color][depth][slot + 1] = _killerMoves[color][depth][slot];
            }

            _killerMoves[color][depth][0] = move;
        }

        public static bool KillerMoveExists(Move move, int color, int depth)
        {
            for (var slot = 0; slot < MoveOrderingConstants.KillerSlots; slot++)
            {
                if (_killerMoves[color][depth][slot] == move)
                {
                    return true;
                }
            }

            return false;
        }

        public static void Clear()
        {
            for (var color = 0; color < 2; color++)
            {
                for (var depth = 0; depth < SearchConstants.MaxDepth; depth++)
                {
                    Array.Clear(_killerMoves[color][depth], 0, MoveOrderingConstants.KillerSlots);
                }
            }
        }
    }
}
