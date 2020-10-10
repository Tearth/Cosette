using System.Collections.Generic;
using Cosette.Arbiter.Engine;
using Cosette.Arbiter.Settings;

namespace Cosette.Arbiter.Tournament
{
    public class GameData
    {
        public List<string> HalfMovesDone { get; set; }
        public bool GameIsDone { get; private set; }
        public Color Winner { get; private set; }

        private BestMoveData _lastBestMove;
        private Color _colorToMove;

        public GameData(List<string> opening)
        {
            HalfMovesDone = opening;
            _colorToMove = Color.White;
        }

        public void MakeMove(BestMoveData bestMoveData)
        {
            HalfMovesDone.Add(bestMoveData.BestMove);
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
            if (HalfMovesDone.Count > SettingsLoader.Data.MaxMovesCount * 2)
            {
                return true;
            }

            if (HalfMovesDone.Count > 8)
            {
                return HalfMovesDone[^1] == HalfMovesDone[^5] && HalfMovesDone[^5] == HalfMovesDone[^9];
            }

            return false;
        }
    }
}
