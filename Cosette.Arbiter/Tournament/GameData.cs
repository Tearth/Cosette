using System.Collections.Generic;
using Cosette.Arbiter.Engine;

namespace Cosette.Arbiter.Tournament
{
    public class GameData
    {
        public List<string> MovesDone { get; set; }
        public bool GameIsDone { get; private set; }
        public Color Winner { get; private set; }

        private BestMoveData _lastBestMove;
        private Color _colorToMove;

        public GameData()
        {
            MovesDone = new List<string>();
            _colorToMove = Color.White;
        }

        public void MakeMove(BestMoveData bestMoveData)
        {
            MovesDone.Add(bestMoveData.BestMove);
            _lastBestMove = bestMoveData;
            
            if (IsCheckmate() || bestMoveData.LastInfoData.ScoreCp >= 2000)
            {
                GameIsDone = true;
                Winner = _colorToMove;
                return;
            }

            if (IsDraw())
            {
                GameIsDone = true;
                Winner = Color.None;
                return;
            }

            _colorToMove = _colorToMove == Color.White ? Color.Black : Color.White;
        }

        public bool IsCheckmate()
        {
            return _lastBestMove != null && _lastBestMove.LastInfoData.ScoreMate == 1;
        }

        public bool IsDraw()
        {
            if (MovesDone.Count > 200)
            {
                return true;
            }

            if (MovesDone.Count > 8)
            {
                return MovesDone[^1] == MovesDone[^5] && MovesDone[^5] == MovesDone[^9];
            }

            return false;
        }
    }
}
