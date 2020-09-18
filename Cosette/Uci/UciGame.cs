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
        public BoardState BoardState;
        private Color _currentColor;
        private int _currentMoveNumber;

        public UciGame()
        {
            BoardState = new BoardState();
        }

        public void SetDefaultState()
        {
            BoardState.SetDefaultState();
            _currentColor = Color.White;
            _currentMoveNumber = 1;
        }

        public void SetFen(string fen)
        {
            BoardState = FenParser.Parse(fen, out _currentMoveNumber);
        }

        public void MakeMove(Move move)
        {
            BoardState.MakeMove(move);
            if (BoardState.ColorToMove == Color.White)
            {
                _currentMoveNumber++;
            }
        }

        public Move SearchBestMove(int whiteTime, int blackTime, int depth)
        {
            var remainingTime = _currentColor == Color.White ? whiteTime : blackTime;
            var bestMove = IterativeDeepening.FindBestMove(BoardState, remainingTime, depth, _currentMoveNumber);

            return bestMove;
        }

        public void SetCurrentColor(Color color)
        {
            _currentColor = color;
        }
    }
}
