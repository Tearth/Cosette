using System;

namespace Cosette.Polyglot.Book
{
    public class PolyglotBoard
    {
        private PieceType[,] _state;
        private CastlingFlags _castlingFlags;
        private ColorType _colorToMove;
        private int _enPassantFile;

        public PolyglotBoard()
        {
            _state = new PieceType[8,8];
            _castlingFlags = CastlingFlags.Everything;
            _colorToMove = ColorType.White;
            _enPassantFile = -1;
        }

        public void InitDefaultState()
        {
            for (var file = 0; file < 8; file++)
            {
                for (var rank = 0; rank < 8; rank++)
                {
                    _state[file, rank] = PieceType.None;
                }
            }

            _state[0, 0] = PieceType.WhiteRook;
            _state[1, 0] = PieceType.WhiteKnight;
            _state[2, 0] = PieceType.WhiteBishop;
            _state[3, 0] = PieceType.WhiteQueen;
            _state[4, 0] = PieceType.WhiteKing;
            _state[5, 0] = PieceType.WhiteBishop;
            _state[6, 0] = PieceType.WhiteKnight;
            _state[7, 0] = PieceType.WhiteRook;

            _state[0, 1] = PieceType.WhitePawn;
            _state[1, 1] = PieceType.WhitePawn;
            _state[2, 1] = PieceType.WhitePawn;
            _state[3, 1] = PieceType.WhitePawn;
            _state[4, 1] = PieceType.WhitePawn;
            _state[5, 1] = PieceType.WhitePawn;
            _state[6, 1] = PieceType.WhitePawn;
            _state[7, 1] = PieceType.WhitePawn;

            _state[0, 6] = PieceType.BlackPawn;
            _state[1, 6] = PieceType.BlackPawn;
            _state[2, 6] = PieceType.BlackPawn;
            _state[3, 6] = PieceType.BlackPawn;
            _state[4, 6] = PieceType.BlackPawn;
            _state[5, 6] = PieceType.BlackPawn;
            _state[6, 6] = PieceType.BlackPawn;
            _state[7, 6] = PieceType.BlackPawn;

            _state[0, 7] = PieceType.BlackRook;
            _state[1, 7] = PieceType.BlackKnight;
            _state[2, 7] = PieceType.BlackBishop;
            _state[3, 7] = PieceType.BlackQueen;
            _state[4, 7] = PieceType.BlackKing;
            _state[5, 7] = PieceType.BlackBishop;
            _state[6, 7] = PieceType.BlackKnight;
            _state[7, 7] = PieceType.BlackRook;
        }

        public void MakeMove(string move)
        {
            var (fromFile, fromRank) = (move[0] - 'a', move[1] - '1');
            var (toFile, toRank) = (move[2] - 'a', move[3] - '1');
            var pieceType = _state[fromFile, fromRank];
            var oldEnPassant = _enPassantFile;

            // Pieces
            _state[toFile, toRank] = _state[fromFile, fromRank];
            _state[fromFile, fromRank] = PieceType.None;
            _enPassantFile = -1;

            // Castling
            if (pieceType == PieceType.WhiteKing)
            {
                _castlingFlags &= ~CastlingFlags.WhiteCastling;

                if (Math.Abs(fromFile - toFile) == 2)
                {
                    // Short castling
                    if (fromFile < toFile)
                    {
                        _state[7, 0] = PieceType.None;
                        _state[5, 0] = PieceType.WhiteRook;
                    }
                    // Long castling
                    else
                    {
                        _state[0, 0] = PieceType.None;
                        _state[3, 0] = PieceType.WhiteRook;
                    }
                }
            }
            else if (pieceType == PieceType.BlackKing)
            {
                _castlingFlags &= ~CastlingFlags.BlackCastling;

                if (Math.Abs(fromFile - toFile) == 2)
                {
                    // Short castling
                    if (fromFile < toFile)
                    {
                        _state[7, 7] = PieceType.None;
                        _state[5, 7] = PieceType.BlackRook;
                    }
                    // Long castling
                    else
                    {
                        _state[0, 7] = PieceType.None;
                        _state[3, 7] = PieceType.BlackRook;
                    }
                }
            }
            else if (pieceType == PieceType.WhiteRook && fromFile == 0 && fromRank == 0)
            {
                _castlingFlags &= ~CastlingFlags.WhiteLong;
            }
            else if (pieceType == PieceType.WhiteRook && fromFile == 7 && fromRank == 0)
            {
                _castlingFlags &= ~CastlingFlags.WhiteShort;
            }
            else if (pieceType == PieceType.BlackRook && fromFile == 0 && fromRank == 7)
            {
                _castlingFlags &= ~CastlingFlags.BlackLong;
            }
            else if (pieceType == PieceType.BlackRook && fromFile == 7 && fromRank == 7)
            {
                _castlingFlags &= ~CastlingFlags.BlackShort;
            }

            // En passant
            if (pieceType == PieceType.WhitePawn || pieceType == PieceType.BlackPawn)
            {
                if (Math.Abs(fromRank - toRank) == 2)
                {
                    var targetPieceType = _colorToMove == ColorType.White ? PieceType.BlackPawn : PieceType.WhitePawn;
                    if (toFile > 0 && _state[toFile - 1, toRank] == targetPieceType || 
                        toFile < 7 && _state[toFile + 1, toRank] == targetPieceType)
                    {
                        _enPassantFile = toFile;
                    }
                }
                else if (Math.Abs(fromFile - toFile) == 1 && toFile == oldEnPassant)
                {
                    if (_colorToMove == ColorType.White)
                    {
                        _state[toFile, toRank - 1] = PieceType.None;
                    }
                    else if (_colorToMove == ColorType.Black)
                    {
                        _state[toFile, toRank + 1] = PieceType.None;
                    }
                }
            }

            // Color
            _colorToMove = _colorToMove == ColorType.White ? ColorType.Black : ColorType.White;
        }

        public ulong CalculateHash()
        {
            ulong result = 0;

            for (var file = 0; file < 8; file++)
            {
                for (var rank = 0; rank < 8; rank++)
                {
                    if (_state[file, rank] != PieceType.None)
                    {
                        result ^= PolyglotConstants.Keys[64 * (int)_state[file, rank] + 8 * rank + file];
                    }
                }
            }

            if ((_castlingFlags & CastlingFlags.WhiteShort) != 0)
            {
                result ^= PolyglotConstants.Keys[768];
            }
            if ((_castlingFlags & CastlingFlags.WhiteLong) != 0)
            {
                result ^= PolyglotConstants.Keys[769];
            }
            if ((_castlingFlags & CastlingFlags.BlackShort) != 0)
            {
                result ^= PolyglotConstants.Keys[770];
            }
            if ((_castlingFlags & CastlingFlags.BlackLong) != 0)
            {
                result ^= PolyglotConstants.Keys[771];
            }

            if (_enPassantFile != -1)
            {
                result ^= PolyglotConstants.Keys[772 + _enPassantFile];
            }

            if (_colorToMove == ColorType.White)
            {
                result ^= PolyglotConstants.Keys[780];
            }

            return result;
        }
    }
}
