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

        public int WhiteClock { get; private set; }
        public int BlackClock { get; private set; }

        private BestMoveData _lastBestMove;
        private Color _colorToMove;

        public GameData(List<string> opening)
        {
            HalfMovesDone = new List<string>(opening);
            WhiteClock = SettingsLoader.Data.BaseTime;
            BlackClock = SettingsLoader.Data.BaseTime;

            _colorToMove = Color.White;
        }

        public void MakeMove(BestMoveData bestMoveData)
        {
            HalfMovesDone.Add(bestMoveData.BestMove);
            _lastBestMove = bestMoveData;

            var clock = _colorToMove == Color.White ? WhiteClock : BlackClock;
            clock -= bestMoveData.LastInfoData.Time;

            if (clock <= 0)
            {
                GameIsDone = true;
                WhiteClock = _colorToMove == Color.White ? clock : WhiteClock;
                BlackClock = _colorToMove == Color.Black ? clock : BlackClock;

                Winner = _colorToMove == Color.White ? Color.Black : Color.White;
                return;
            }

            clock += SettingsLoader.Data.IncTime;

            WhiteClock = _colorToMove == Color.White ? clock : WhiteClock;
            BlackClock = _colorToMove == Color.Black ? clock : BlackClock;

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
