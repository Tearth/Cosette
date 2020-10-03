using System;
using Cosette.Arbiter.Engine;
using Cosette.Arbiter.Logs;
using Cosette.Arbiter.Settings;

namespace Cosette.Arbiter.Tournament
{
    public class TournamentArbiter
    {
        private EngineOperator _engine1 { get; set; }
        private EngineOperator _engine2 { get; set; }

        public TournamentArbiter()
        {
            _engine1 = new EngineOperator(SettingsLoader.Data.Engine1Name, SettingsLoader.Data.Engine1Path);
            _engine2 = new EngineOperator(SettingsLoader.Data.Engine2Name, SettingsLoader.Data.Engine2Path);
        }

        public void Run()
        {
            _engine1.Init();
            _engine2.Init();

            for (var gameIndex = 0; gameIndex < SettingsLoader.Data.GamesCount; gameIndex++)
            {
                LogManager.Log($"Game {gameIndex}: ");
                var gameData = new GameData();

                _engine1.InitNewGame();
                _engine2.InitNewGame();

                var currentEngineToMove = _engine1;
                while (!gameData.IsOver())
                {
                    var bestMoveData = currentEngineToMove.Go(gameData.MovesDone);
                    gameData.MakeMove(bestMoveData);

                    LogManager.Log(bestMoveData.BestMove);
                    LogManager.Log(" ");
                    currentEngineToMove = currentEngineToMove == _engine1 ? _engine2 : _engine1;
                }

                if (gameData.IsDraw())
                {
                    LogManager.LogLine($" === Result: draw");
                }
                else if (gameData.IsCheckmate())
                {
                    LogManager.LogLine($" === Result: {gameData.GetWinner()} won");
                }
            }
        }
    }
}
