using System;
using System.Diagnostics;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Perft.Results;

namespace Cosette.Engine.Perft
{
    public static class VerificationPerft
    {
        public static VerificationPerftResult Run(BoardState boardState, Color color, int depth)
        {
            var verificationSuccess = true;
            var leafsCount = Perft(boardState, color, depth, ref verificationSuccess);

            return new VerificationPerftResult
            {
                LeafsCount = leafsCount,
                VerificationSuccess = verificationSuccess
            };
        }

        private static ulong Perft(BoardState boardState, Color color, int depth, ref bool verificationSuccess)
        {
            if (!VerifyBoard(boardState))
            {
                verificationSuccess = false;
            }

            if (depth <= 0)
            {
                return 1;
            }

            Span<Move> moves = stackalloc Move[128];
            var movesCount = boardState.GetAvailableMoves(moves, color);

            ulong nodes = 0;
            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i], color);
                if (!boardState.IsKingChecked(color))
                {
                    nodes += Perft(boardState, ColorOperations.Invert(color), depth - 1, ref verificationSuccess);
                }
                boardState.UndoMove(moves[i], color);
            }

            return nodes;
        }

        private static bool VerifyBoard(BoardState board)
        {
            if (board.Material != board.CalculateMaterial(Color.White) + board.CalculateMaterial(Color.Black))
            {
                return false;
            }

            return true;
        }
    }
}