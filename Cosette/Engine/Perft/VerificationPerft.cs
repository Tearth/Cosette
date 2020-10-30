using System;
using Cosette.Engine.Ai.Search;
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
                return 0;
            }

            if (depth <= 0)
            {
                return 1;
            }

            Span<Move> moves = stackalloc Move[SearchConstants.MaxMovesCount];
            var movesCount = boardState.GetAvailableMoves(moves);

            ulong nodes = 0;
            for (var i = 0; i < movesCount; i++)
            {
                boardState.MakeNullMove();
                boardState.UndoNullMove();

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
            if (board.Material[Color.White] != board.CalculateMaterial(Color.White) ||
                board.Material[Color.Black] != board.CalculateMaterial(Color.Black))
            {
                return false;
            }

            if (board.Hash != ZobristHashing.CalculateHash(board))
            {
                return false;
            }

            if (board.PawnHash != ZobristHashing.CalculatePawnHash(board))
            {
                return false;
            }

            if (board.Position[Color.White][GamePhase.Opening] != board.CalculatePosition(Color.White, GamePhase.Opening) ||
                board.Position[Color.White][GamePhase.Ending] != board.CalculatePosition(Color.White, GamePhase.Ending) ||
                board.Position[Color.Black][GamePhase.Opening] != board.CalculatePosition(Color.Black, GamePhase.Opening) ||
                board.Position[Color.Black][GamePhase.Ending] != board.CalculatePosition(Color.Black, GamePhase.Ending))
            {
                return false;
            }

            var pieceTable = new int[64];
            board.CalculatePieceTable(pieceTable);

            for (var fieldIndex = 0; fieldIndex < 64; fieldIndex++)
            {
                if (board.PieceTable[fieldIndex] != pieceTable[fieldIndex])
                {
                    return false;
                }
            }

            return true;
        }
    }
}