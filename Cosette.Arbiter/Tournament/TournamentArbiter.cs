using System;
using System.Collections.Generic;
using Cosette.Arbiter.Book;
using Cosette.Arbiter.Engine;
using Cosette.Arbiter.Settings;

namespace Cosette.Arbiter.Tournament
{
    public class TournamentArbiter
    {
        private List<TournamentParticipant> _participants;
        private TournamentScheduler _scheduler;
        private PolyglotBook _polyglotBook;
        private int _errors;

        public TournamentArbiter()
        {
            _participants = new List<TournamentParticipant>();
            _scheduler = new TournamentScheduler();
            _polyglotBook = new PolyglotBook();
            
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
                var gameData = new GameData(_polyglotBook.GetRandomOpening());
                var (playerA, playerB) = _scheduler.GetPair(gameIndex);

                if (playerA >= _participants.Count || playerB >= _participants.Count)
                {
                    continue;
                }

                var participantA = _participants[playerA];
                var participantB = _participants[playerB];

                var whitePlayer = DateTime.UtcNow.Ticks % 2 == 0 ? participantA : participantB;
                var blackPlayer = whitePlayer == participantA ? participantB : participantA;
                var (playerToMove, opponent) = (whitePlayer, blackPlayer);

                Console.Clear();
                WriteResults();

                Console.WriteLine();
                Console.WriteLine($"Game {gameIndex} ({whitePlayer.EngineData.Name} vs. {blackPlayer.EngineData.Name})");
                Console.Write("Moves: ");
                Console.Write(string.Join(' ', gameData.HalfMovesDone));
                Console.Write(" ");

                participantA.EngineOperator.InitNewGame();
                participantB.EngineOperator.InitNewGame();


                while (true)
                {
                    var bestMoveData = playerToMove.EngineOperator.Go(gameData.HalfMovesDone);
                    if (bestMoveData == null)
                    {
                        _errors++;
                        break;
                    }

                    gameData.MakeMove(bestMoveData);

                    Console.Write(bestMoveData.BestMove);
                    Console.Write(" ");

                    if (gameData.GameIsDone)
                    {
                        if (gameData.Winner == Color.None)
                        {
                            playerToMove.History.Add(new ArchivedGame(gameData, opponent, GameResult.Draw));
                            opponent.History.Add(new ArchivedGame(gameData, playerToMove, GameResult.Draw));
                        }
                        else
                        {
                            playerToMove.History.Add(new ArchivedGame(gameData, opponent, GameResult.Win));
                            opponent.History.Add(new ArchivedGame(gameData, playerToMove, GameResult.Loss));
                        }

                        break;
                    }

                    (playerToMove, opponent) = (opponent, playerToMove);
                }
            }
        }

        private void WriteResults()
        {
            foreach (var participant in _participants)
            {
                var originalRating = participant.EngineData.Rating;
                var performance = participant.CalculatePerformanceRating() - originalRating;

                Console.WriteLine($"{participant.EngineData.Name} {originalRating} ELO ({performance:+0;-#}): " +
                                  $"{participant.Wins} wins, {participant.Losses} losses, " +
                                  $"{participant.Draws} draws, {_errors} errors");
            }
        }
    }
}
