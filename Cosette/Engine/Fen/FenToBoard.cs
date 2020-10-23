using System;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Fen
{
    public static class FenToBoard
    {
        public static BoardState Parse(string fen)
        {
            var split = fen.Split(' ');
            var boardState = split[0];
            var colorState = split[1];
            var castlingState = split[2];
            var enPassantState = split[3];
            var halfmoveClock = split.Length > 4 ? int.Parse(split[4]) : 0;
            var movesCount = split.Length > 5 ? int.Parse(split[5]) : 0;

            var result = new BoardState();
            var currentColor = ParseCurrentColor(colorState);

            ParseBoardState(boardState, result);
            ParseCastlingState(castlingState, result);
            ParseEnPassantState(enPassantState, result);

            result.Material[Color.White] = result.CalculateMaterial(Color.White);
            result.Material[Color.Black] = result.CalculateMaterial(Color.Black);

            result.Position[Color.White][GamePhase.Opening] = result.CalculatePosition(Color.White, GamePhase.Opening);
            result.Position[Color.White][GamePhase.Ending] = result.CalculatePosition(Color.White, GamePhase.Ending);
            result.Position[Color.Black][GamePhase.Opening] = result.CalculatePosition(Color.Black, GamePhase.Opening);
            result.Position[Color.Black][GamePhase.Ending] = result.CalculatePosition(Color.Black, GamePhase.Ending);

            result.MovesCount = movesCount;
            result.IrreversibleMovesCount = halfmoveClock;
            result.ColorToMove = currentColor;
            result.Hash = ZobristHashing.CalculateHash(result);
            result.PawnHash = ZobristHashing.CalculatePawnHash(result);

            return result;
        }

        private static void ParseBoardState(string boardState, BoardState result)
        {
            var ranks = boardState.Split('/');
            var position = new Position(0, 7);

            for (var rank = 0; rank < 8; rank++)
            {
                for (var file = 0; file < ranks[rank].Length; file++)
                {
                    var c = ranks[rank][file];
                    if (char.IsLetter(c))
                    {
                        var piece = ConvertToPiece(c);
                        var color = ConvertToColor(c);
                        result.AddPiece(color, piece, (byte)position.ToFieldIndex());
                        position += new Position(1, 0);
                    }
                    else if (char.IsDigit(c))
                    {
                        position += new Position(c - '0', 0);
                    }
                }

                position = new Position(0, position.Y - 1);
            }
        }

        private static int ParseCurrentColor(string currentColor)
        {
            return currentColor == "w" ? Color.White : Color.Black;
        }

        private static void ParseCastlingState(string castlingState, BoardState result)
        {
            if (castlingState.Contains('K'))
            {
                result.Castling |= Castling.WhiteShort;
            }

            if (castlingState.Contains('Q'))
            {
                result.Castling |= Castling.WhiteLong;
            }

            if (castlingState.Contains('k'))
            {
                result.Castling |= Castling.BlackShort;
            }

            if (castlingState.Contains('q'))
            {
                result.Castling |= Castling.BlackLong;
            }
        }

        private static void ParseEnPassantState(string enPassantState, BoardState result)
        {
            if (enPassantState != "-")
            {
                var position = Position.FromText(enPassantState);
                result.EnPassant = 1ul << position.ToFieldIndex();
            }
        }

        private static int ConvertToPiece(char c)
        {
            switch (char.ToLower(c))
            {
                case 'p': return Piece.Pawn;
                case 'r': return Piece.Rook;
                case 'n': return Piece.Knight;
                case 'b': return Piece.Bishop;
                case 'q': return Piece.Queen;
                case 'k': return Piece.King;
            }

            throw new InvalidOperationException();
        }

        private static int ConvertToColor(char c)
        {
            return char.IsUpper(c) ? Color.White : Color.Black;
        }
    }
}
