using System.Runtime.CompilerServices;
using Cosette.Engine.Moves.Magic;

namespace Cosette.Engine.Moves
{
    public static class RookMovesGenerator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetMoves(ulong board, int fieldIndex)
        {
            return MagicBitboards.GetRookMoves(board, fieldIndex);
        }
    }
}