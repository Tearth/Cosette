using System;
using System.Collections.Generic;
using Cosette.Arbiter.Engine;

namespace Cosette.Arbiter.Tournament
{
    public class GameData
    {
        public List<string> MovesDone { get; set; }

        private BestMoveData _lastBestMove;
        private Color _colorToMove;

        private bool _gameIsDone;
        private Color _winner;

        public GameData()
        {
            MovesDone = new List<string>();
            _colorToMove = Color.White;
        }

        public void MakeMove(BestMoveData bestMoveData)
        {
            MovesDone.Add(bestMoveData.BestMove);
            _lastBestMove = bestMoveData;

            if (IsCheckmate())
            {
                _gameIsDone = true;
                _winner = _colorToMove;
            }

            _colorToMove = _colorToMove == Color.White ? Color.Black : Color.White;
        }

        public bool IsOver()
        {
            return IsCheckmate() || IsDraw();
        }

        public bool IsCheckmate()
        {
            if (_lastBestMove != null)
            {
                var scoreAbs = Math.Abs(_lastBestMove.Score);
                if (scoreAbs >= 31999 && scoreAbs <= 32001)
                {
                    return true;
                }
            }

            return false;
        }

        public bool IsDraw()
        {
            if (_lastBestMove.IrreversibleMovesCount >= 100)
            {
                return true;
            }

            if (MovesDone.Count > 8)
            {
                return MovesDone[0] == MovesDone[4] && MovesDone[4] == MovesDone[8];
            }

            return false;
        }
    }
}
