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
        public static VerificationPerftResult Run(BoardState boardState, int depth)
        {
            var verificationSuccess = true;
            var leafsCount = Perft(boardState, depth, ref verificationSuccess);

            return new VerificationPerftResult
            {
                LeafsCount = leafsCount,
                VerificationSuccess = verificationSuccess
            };
        }

        private static ulong Perft(BoardState boardState, int depth, ref bool verificationSuccess)
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
            var movesCount = boardState.GetAvailableMoves(moves);

            ulong nodes = 0;
            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeMove(moves[i]);
                if (!boardState.IsKingChecked(ColorOperations.Invert(boardState.ColorToMove)))
                {
                    nodes += Perft(boardState, depth - 1, ref verificationSuccess);
                }
                boardState.UndoMove(moves[i]);
            }

            return nodes;
        }

        private static bool VerifyBoard(BoardState board)
        {
            if (board.Material[(int)Color.White] != board.CalculateMaterial(Color.White) ||
                board.Material[(int)Color.Black] != board.CalculateMaterial(Color.Black))
            {
                return false;
            }

            if (board.Hash != ZobristHashing.CalculateHash(board))
            {
                return false;
            }

            if (board.Position[(int) Color.White][(int) GamePhase.Opening] != board.CalculatePosition(Color.White, GamePhase.Opening) ||
                board.Position[(int) Color.White][(int) GamePhase.Ending] != board.CalculatePosition(Color.White, GamePhase.Ending) ||
                board.Position[(int) Color.Black][(int) GamePhase.Opening] != board.CalculatePosition(Color.Black, GamePhase.Opening) ||
                board.Position[(int) Color.Black][(int) GamePhase.Ending] != board.CalculatePosition(Color.Black, GamePhase.Ending))
            {
                return false;
            }

            return true;
        }
    }
}