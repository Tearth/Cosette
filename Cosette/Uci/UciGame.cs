using System;
using System.Threading.Tasks;
using Cosette.Engine.Ai;
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

        public UciGame()
        {
            _boardState = new BoardState();
        }

        public void SetDefaultState()
        {
            _boardState.SetDefaultState();
            _currentColor = Color.White;
        }

        public void SetFen(string fen)
        {
            _boardState = FenParser.Parse(fen, out _currentColor);
        }

        public bool MakeMove(Color color, Position from, Position to)
        {
            Span<Move> moves = stackalloc Move[128];
            var movesCount = _boardState.GetAvailableMoves(moves, color);

            for (var i = 0; i < movesCount; i++)
            {
                if (Position.FromFieldIndex(moves[i].From) == from && Position.FromFieldIndex(moves[i].To) == to)
                {
                    _boardState.MakeMove(moves[i], color);
                    return true;
                }
            }

            return false;
        }

        public Move SearchBestMove()
        {
            var statistics = new SearchStatistics();
            var score = NegaMax.FindBestMove(_boardState, _currentColor, 4, out Move bestMove, statistics);

            return bestMove;
        }

        public void SetCurrentColor(Color color)
        {
            _currentColor = color;
        }
    }
}
