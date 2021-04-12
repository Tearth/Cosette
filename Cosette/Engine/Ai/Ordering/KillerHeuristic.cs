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

            for (var ply = 0; ply < SearchConstants.MaxDepth; ply++)
            {
                _killerMoves[Color.White][ply] = new Move[MoveOrderingConstants.KillerSlots];
                _killerMoves[Color.Black][ply] = new Move[MoveOrderingConstants.KillerSlots];
            }
        }

        public static void AddKillerMove(Move move, int color, int ply)
        {
            for (var slot = MoveOrderingConstants.KillerSlots - 2; slot >= 0; slot--)
            {
                _killerMoves[color][ply][slot + 1] = _killerMoves[color][ply][slot];
            }

            _killerMoves[color][ply][0] = move;
        }

        public static bool KillerMoveExists(Move move, int color, int ply)
        {
            for (var slot = 0; slot < MoveOrderingConstants.KillerSlots; slot++)
            {
                if (_killerMoves[color][ply][slot] == move)
                {
                    return true;
                }
            }

            return false;
        }

        public static void AgeKillers()
        {
            for (var color = 0; color < 2; color++)
            {
                for (var ply = 0; ply < SearchConstants.MaxDepth - 2; ply++)
                {
                    _killerMoves[color][ply] = _killerMoves[color][ply + 2];
                }
            }

            for (var color = 0; color < 2; color++)
            {
                for (var ply = SearchConstants.MaxDepth - 2; ply < SearchConstants.MaxDepth; ply++)
                {
                    _killerMoves[color][ply] = new Move[MoveOrderingConstants.KillerSlots];
                }
            }
        }

        public static void Clear()
        {
            for (var color = 0; color < 2; color++)
            {
                for (var ply = 0; ply < SearchConstants.MaxDepth; ply++)
                {
                    Array.Clear(_killerMoves[color][ply], 0, MoveOrderingConstants.KillerSlots);
                }
            }
        }
    }
}
