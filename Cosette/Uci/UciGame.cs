using System;
using System.Threading.Tasks;
using Cosette.Engine.Ai;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Fen;
using Cosette.Engine.Moves;

namespace Cosette.Uci
{
    public class UciGame
    {
        private BoardState _boardState;
        private Color _currentColor;
        private int _currentMoveNumber;

        public UciGame()
        {
            _boardState = new BoardState();
        }

        public void SetDefaultState()
        {
            _boardState.SetDefaultState();
            _currentColor = Color.White;
            _currentMoveNumber = 1;
        }

        public void SetFen(string fen)
        {
            _boardState = FenParser.Parse(fen, out _currentMoveNumber);
        }

        public bool MakeMove(Position from, Position to, MoveFlags flags)
        {
            Span<Move> moves = stackalloc Move[128];
            var movesCount = _boardState.GetAvailableMoves(moves);

            for (var i = 0; i < movesCount; i++)
            {
                if (Position.FromFieldIndex(moves[i].From) == from && Position.FromFieldIndex(moves[i].To) == to)
                {
                    if (flags == MoveFlags.None || (moves[i].Flags & flags) != 0)
                    {
                        _boardState.MakeMove(moves[i]);
                        if (_boardState.ColorToMove == Color.White)
                        {
                            _currentMoveNumber++;
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public Move SearchBestMove(int whiteTime, int blackTime)
        {
            GC.TryStartNoGCRegion(1024 * 1024 * 16);

            var remainingTime = _currentColor == Color.White ? whiteTime : blackTime;
            var bestMove = IterativeDeepening.FindBestMove(_boardState, remainingTime, _currentMoveNumber);

            GC.EndNoGCRegion();

            return bestMove;
        }

        public void SetCurrentColor(Color color)
        {
            _currentColor = color;
        }
    }
}
