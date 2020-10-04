using System;
using System.Collections.Generic;
using Cosette.Arbiter.Engine;
using Cosette.Arbiter.Settings;

namespace Cosette.Arbiter.Tournament
{
    public class TournamentArbiter
    {
        private List<TournamentParticipant> _participants;
        private TournamentScheduler _scheduler;

        public TournamentArbiter()
        {
            _participants = new List<TournamentParticipant>();
            _scheduler = new TournamentScheduler();
            
            foreach (var engineData in SettingsLoader.Data.Engines)
            {
                var engineOperator = new EngineOperator(engineData.Name, engineData.Path);
                var tournamentParticipant = new TournamentParticipant(engineData, engineOperator);

                _participants.Add(tournamentParticipant);
            }

            _scheduler.Init(_participants.Count);
        }

        public void Run()
        {
            _participants.ForEach(p => p.EngineOperator.Init());
            for (var gameIndex = 0; gameIndex < SettingsLoader.Data.GamesCount; gameIndex++)
            {
                var gameData = new GameData();
                var (playerA, playerB) = _scheduler.GetPair(gameIndex);
                var participantA = _participants[playerA];
                var participantB = _participants[playerB];

                Console.Clear();
                WriteResults();

                Console.WriteLine();
                Console.WriteLine($"Game {gameIndex}");
                Console.Write("Moves: ");

                participantA.EngineOperator.InitNewGame();
                participantB.EngineOperator.InitNewGame();

                var currentEngineToMove = DateTime.UtcNow.Ticks % 2 == 0 ? participantA : participantB;
                while (true)
                {
                    var bestMoveData = currentEngineToMove.EngineOperator.Go(gameData.MovesDone);
                    gameData.MakeMove(bestMoveData);

                    Console.Write(bestMoveData.BestMove);
                    Console.Write(" ");

                    if (gameData.GameIsDone)
                    {
                        currentEngineToMove.Wins++;
                        break;
                    }
                    else if (gameData.IsDraw())
                    {
                        _participants[playerA].Draws++;
                        _participants[playerB].Draws++;
                        break;
                    }

                    currentEngineToMove = currentEngineToMove == participantA ? participantB : participantA;
                }
            }
        }

        private void WriteResults()
        {
            foreach (var participant in _participants)
            {
                Console.WriteLine($"{participant.EngineData.Name}: {participant.Wins} wins, {participant.Losses} losses, {participant.Draws} draws");
            }
        }
    }
}
