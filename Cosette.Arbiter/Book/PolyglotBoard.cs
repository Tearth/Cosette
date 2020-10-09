namespace Cosette.Arbiter.Book
{
    public class PolyglotBoard
    {
        private PieceType[,] _state;

        public PolyglotBoard()
        {
            _state = new PieceType[8,8];
        }

        public void InitDefaultState()
        {
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

            return result;
        }
    }
}
