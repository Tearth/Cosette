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

        public TournamentArbiter()
        {
            _participants = new List<TournamentParticipant>();
            _gamesDuration = new List<long>();
            _scheduler = new TournamentScheduler();
            _polyglotBook = new PolyglotBook(SettingsLoader.Data.PolyglotOpeningBook);
            
            foreach (var engineData in SettingsLoader.Data.Engines)
            {
                var engineOperator = new EngineOperator(engineData.Path, engineData.Arguments);
                var tournamentParticipant = new TournamentParticipant(engineData, engineOperator);

                _participants.Add(tournamentParticipant);
            }

            _scheduler.Init(_participants.Count, SettingsLoader.Data.Gauntlet);
        }

        public void Run()
        {
            _participants.ForEach(p => p.EngineOperator.Init());
            for (var gameIndex = 0; gameIndex < SettingsLoader.Data.GamesCount;)
            {
                var openingBookMoves = _polyglotBook.GetRandomOpening(SettingsLoader.Data.PolyglotMaxMoves);
                var (playerA, playerB) = _scheduler.GetPair(gameIndex / 2);

                if (playerA >= _participants.Count || playerB >= _participants.Count)
                {
                    gameIndex++;
                    continue;
                }

                var whitePlayer = _participants[playerA];
                var blackPlayer = _participants[playerB];

                for (var i = 0; i < 2; i++)
                {
                    var gameData = new GameData(openingBookMoves);
                    var (playerToMove, opponent) = (whitePlayer, blackPlayer);

                    Console.Clear();
                    WriteResults();
                    WriteTournamentStatistics();

                    Console.WriteLine($"Game {gameIndex} ({whitePlayer.EngineData.Name} vs. {blackPlayer.EngineData.Name}), {openingBookMoves.Count} opening book moves:");
                    Console.Write("Moves: ");
                    Console.Write(string.Join(' ', gameData.HalfMovesDone));
                    Console.Write(" ");

                    whitePlayer.EngineOperator.InitNewGame();
                    blackPlayer.EngineOperator.InitNewGame();

                    var gameStopwatch = Stopwatch.StartNew();
                    while (true)
                    {
                        try
                        {
                            var bestMoveData = playerToMove.EngineOperator.Go(gameData.HalfMovesDone, gameData.WhiteClock, gameData.BlackClock);
                            if (bestMoveData == null || bestMoveData.LastInfoData == null || bestMoveData.BestMove == "h1h1")
                            {
                                playerToMove.History.Add(new ArchivedGame(gameData, opponent, GameResult.Draw));
                                opponent.History.Add(new ArchivedGame(gameData, playerToMove, GameResult.Draw));
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
                                    playerToMove.History.Add(new ArchivedGame(gameData, opponent, GameResult.Loss, true));
                                    opponent.History.Add(new ArchivedGame(gameData, playerToMove, GameResult.Win, true));
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
                        catch
                        {
                            whitePlayer.EngineOperator.Restart();
                            blackPlayer.EngineOperator.Restart();
                        }
                    }

                    _gamesDuration.Add(gameStopwatch.ElapsedMilliseconds);
                    (whitePlayer, blackPlayer) = (blackPlayer, whitePlayer);
                    gameIndex++;
                }
            }

            Console.WriteLine();
            Console.WriteLine("Test ended, press any key to close Arbiter");
            Console.ReadLine();
        }

        private void WriteResults()
        {
            foreach (var participant in _participants)
            {
                var performance = participant.CalculatePerformanceRating();
                var wonGamesPercent = participant.WonGamesPercent();
                var winsByTime = participant.History.Count(p => p.Result == GameResult.Win && p.TimeFlag);
                var lossesByTime = participant.History.Count(p => p.Result == GameResult.Loss && p.TimeFlag);

                Console.WriteLine($"{participant.EngineData.Name} ({performance:+0;-#}, {wonGamesPercent}%): " +
                                  $"{participant.Wins} wins ({winsByTime} by time), {participant.Losses} losses ({lossesByTime} by time), " +
                                  $"{participant.Draws} draws");
                Console.WriteLine($" === {participant.AverageDepth:F} average depth, {participant.AverageNodesCount} average nodes, " +
                                  $"{participant.AverageNps} average nodes per second");
                Console.WriteLine($"Executable hash: {participant.EngineOperator.ExecutableHash.Value}");
                Console.WriteLine();
            }
        }

        private void WriteTournamentStatistics()
        {
            var averageGameTime = (_gamesDuration.Count != 0 ? _gamesDuration.Average() : 0.0) / 1000;

            Console.WriteLine($"Tournament statistics: {averageGameTime:F} s per average game");
            Console.WriteLine();
        }
    }
}
