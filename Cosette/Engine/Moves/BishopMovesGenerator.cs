using System.Runtime.CompilerServices;
using Cosette.Engine.Moves.Magic;

namespace Cosette.Engine.Moves
{
    public static class BishopMovesGenerator
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong GetMoves(ulong board, int fieldIndex)
        {
            return MagicBitboards.GetBishopMoves(board, fieldIndex);
        }
    }
}