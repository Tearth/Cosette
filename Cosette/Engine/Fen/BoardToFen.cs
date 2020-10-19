using System;
using System.Net;
using System.Text;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Fen
{
    public static class BoardToFen
    {
        public static string Parse(BoardState board)
        {
            return string.Join(' ',
                ParseBoardState(board),
                ParseColor(board),
                ParseCastling(board),
                ParseEnPassant(board),
                ParseHalfmoveClock(board),
                ParseFullmoveNumber(board)
            );
        }

        private static string ParseBoardState(BoardState board)
        {
            var resultBuilder = new StringBuilder();
            var rankBuilder = new StringBuilder();

            for (var rank = 7; rank >= 0; rank--)
            {
                var emptyFields = 0;

                for (var file = 7; file >= 0; file--)
                {
                    var fieldIndex = rank * 8 + file;
                    
                    var possibleWhitePiece = board.PieceTable[fieldIndex];
                    var possibleBlackPiece = board.PieceTable[fieldIndex];
                    
                    var color = (board.Occupancy[Color.White] & (1ul << fieldIndex)) != 0 ? Color.White :
                                (board.Occupancy[Color.Black] & (1ul << fieldIndex)) != 0 ? Color.Black :
                                -1;

                    var piece = color == Color.White ? possibleWhitePiece :
                                color == Color.Black ? possibleBlackPiece :
                                -1;

                    if (piece != -1)
                    {
                        if (emptyFields != 0)
                        {
                            rankBuilder.Append(emptyFields);
                            emptyFields = 0;
                        }

                        rankBuilder.Append(ConvertToPiece(piece, color));
                    }
                    else
                    {
                        emptyFields++;
                    }
                }

                if (emptyFields != 0)
                {
                    rankBuilder.Append(emptyFields);
                }

                resultBuilder.Append(rankBuilder.ToString());
                rankBuilder.Clear();

                if (rank > 0)
                {
                    resultBuilder.Append('/');
                }
            }

            return resultBuilder.ToString();
        }

        private static string ParseColor(BoardState board)
        {
            return board.ColorToMove == Color.White ? "w" : "b";
        }

        private static string ParseCastling(BoardState board)
        {
            var resultBuilder = new StringBuilder();
            
            if ((board.Castling & Castling.WhiteShort) != 0)
            {
                resultBuilder.Append('K');
            }

            if ((board.Castling & Castling.WhiteLong) != 0)
            {
                resultBuilder.Append('Q');
            }

            if ((board.Castling & Castling.BlackShort) != 0)
            {
                resultBuilder.Append('k');
            }

            if ((board.Castling & Castling.BlackLong) != 0)
            {
                resultBuilder.Append('q');
            }

            if (resultBuilder.Length == 0)
            {
                resultBuilder.Append('-');
            }

            return resultBuilder.ToString();
        }

        private static string ParseEnPassant(BoardState board)
        {
            if (board.EnPassant == 0)
            {
                return "-";
            }

            var enPassantField = BitOperations.BitScan(board.EnPassant);
            var enPassantPosition = Position.FromFieldIndex(enPassantField);

            return enPassantPosition.ToString();
        }

        private static string ParseHalfmoveClock(BoardState board)
        {
            return board.IrreversibleMovesCount.ToString();
        }

        private static string ParseFullmoveNumber(BoardState board)
        {
            return board.MovesCount.ToString();
        }

        private static char ConvertToPiece(int piece, int color)
        {
            var result = piece switch
            {
                Piece.Pawn => 'P',
                Piece.Rook => 'R',
                Piece.Knight => 'N',
                Piece.Bishop => 'B',
                Piece.Queen => 'Q',
                Piece.King => 'K',
                _ => throw new InvalidOperationException()
            };

            return color == Color.Black ? char.ToLower(result) : result;
        }
    }
}
