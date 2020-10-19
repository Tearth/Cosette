using Cosette.Engine.Moves.Magic;

namespace Cosette.Engine.Moves
{
    public static class RookMovesGenerator
    {
        public static ulong GetMoves(ulong board, int fieldIndex)
        {
            return MagicBitboards.GetRookMoves(board, fieldIndex);
        }
    }
}