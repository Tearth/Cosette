using System.Runtime.CompilerServices;
using Cosette.Engine.Moves.Magic;

namespace Cosette.Engine.Moves
{
    public static class QueenMovesGenerator
    {
        public static ulong GetMoves(ulong board, int fieldIndex)
        {
            return MagicBitboards.GetRookMoves(board, fieldIndex) | MagicBitboards.GetBishopMoves(board, fieldIndex);
        }
    }
}