using System.Runtime.CompilerServices;
using Cosette.Engine.Moves.Magic;

namespace Cosette.Engine.Moves
{
    public static class BishopMovesGenerator
    {
#if INLINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static ulong GetMoves(ulong board, int fieldIndex)
        {
            return MagicBitboards.GetBishopMoves(board, fieldIndex);
        }
    }
}