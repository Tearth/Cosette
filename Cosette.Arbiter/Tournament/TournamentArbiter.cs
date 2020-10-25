using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Cosette.Arbiter.Engine;
using Cosette.Arbiter.Settings;
using Cosette.Polyglot;

namespace Cosette.Arbiter.Tournament
{
    public class TournamentArbiter
    {
        private List<TournamentParticipant> _participants;
        private List<long> _gamesDuration;
        private TournamentScheduler _scheduler;
        private PolyglotBook _polyglotBook;
        private int _errors;
        private int _winsByTime;

        public TournamentArbiter()
        {
            _participants = new List<TournamentParticipant>();
            _gamesDuration = new List<long>();
            _scheduler = new TournamentScheduler();
            _polyglotBook = new PolyglotBook(SettingsLoader.Data.PolyglotOpeningBook);
            
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
                var gameData = new GameData(_polyglotBook.GetRandomOpening(SettingsLoader.Data.PolyglotMaxMoves));
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
                WriteTournamentStatistics();

                Console.WriteLine($"Game {gameIndex} ({whitePlayer.EngineData.Name} vs. {blackPlayer.EngineData.Name})");
                Console.Write("Moves: ");
                Console.Write(string.Join(' ', gameData.HalfMovesDone));
                Console.Write(" ");

                participantA.EngineOperator.InitNewGame();
                participantB.EngineOperator.InitNewGame();

                var gameStopwatch = Stopwatch.StartNew();
                while (true)
                {
                    var bestMoveData = playerToMove.EngineOperator.Go(gameData.HalfMovesDone, gameData.WhiteClock, gameData.BlackClock);
                    if (bestMoveData == null)
                    {
                        _errors++;
                        break;
                    }

                    playerToMove.Logs.Add(bestMoveData.LastInfoData);
                    gameData.MakeMove(bestMoveData);

                    Console.Write(bestMoveData.BestMove);
                    Console.Write(" ");

                    if (gameData.GameIsDone)
                    {
                        if (gameData.WhiteClock <= 0 || gameData.BlackClock <= 0)
                        {
                            _winsByTime++;
                        }
                        else if (gameData.Winner == Color.None)
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

                _gamesDuration.Add(gameStopwatch.ElapsedMilliseconds);
            }

            Console.WriteLine();
            Console.WriteLine("Test ended, press any key to close Arbiter");
            Console.ReadLine();
        }

        private void WriteResults()
        {
            foreach (var participant in _participants)
            {
                var originalRating = participant.EngineData.Rating;
                var performance = participant.CalculatePerformanceRating() - originalRating;
                var wonGamesPercent = participant.WonGamesPercent();

                Console.WriteLine($"{participant.EngineData.Name} {originalRating} ELO ({performance:+0;-#}, {wonGamesPercent}%): " +
                                  $"{participant.Wins} wins, {participant.Losses} losses, {participant.Draws} draws");
                Console.WriteLine($" === {participant.AverageDepth:F1} average depth, {participant.AverageNodesCount} average nodes, " +
                                  $"{participant.AverageNps} average nodes per second");
                Console.WriteLine();
            }
        }

        private void WriteTournamentStatistics()
        {
            var averageGameTime = (_gamesDuration.Count != 0 ? _gamesDuration.Average() : 0.0) / 1000;

            Console.WriteLine($"Tournament statistics: {averageGameTime:F} s per average game, {_winsByTime} wins by time, {_errors} errors");
            Console.WriteLine();
        }
    }
}
