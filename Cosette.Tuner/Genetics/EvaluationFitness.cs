using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cosette.Polyglot;
using Cosette.Tuner.Common.Requests;
using Cosette.Tuner.Engine;
using Cosette.Tuner.Genetics.Game;
using Cosette.Tuner.Settings;
using Cosette.Tuner.Web;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Cosette.Tuner.Genetics
{
    public class EvaluationFitness : IFitness
    {
        private int _testId;
        private WebService _webService;
        private EngineOperator _referenceEngineOperator;
        private EngineOperator _experimentalEngineOperator;
        private PolyglotBook _polyglotBook;

        public EvaluationFitness(int testId, WebService webService)
        {
            _testId = testId;
            _webService = webService;
            _referenceEngineOperator = new EngineOperator(SettingsLoader.Data.EnginePath, SettingsLoader.Data.EngineArguments);
            _experimentalEngineOperator = new EngineOperator(SettingsLoader.Data.EnginePath, SettingsLoader.Data.EngineArguments);
            _polyglotBook = new PolyglotBook(SettingsLoader.Data.PolyglotOpeningBook);

            _referenceEngineOperator.Init();
            _experimentalEngineOperator.Init();
        }

        public double Evaluate(IChromosome chromosome)
        {
            var referenceParticipant = new EvaluationParticipant(_referenceEngineOperator);
            var experimentalParticipant = new EvaluationParticipant(_experimentalEngineOperator);
            
            for (var geneIndex = 0; geneIndex < SettingsLoader.Data.Genes.Count; geneIndex++)
            {
                experimentalParticipant.EngineOperator.SetOption(SettingsLoader.Data.Genes[geneIndex].Name, chromosome.GetGene(geneIndex).ToString());
            }

            referenceParticipant.EngineOperator.ApplyOptions();
            experimentalParticipant.EngineOperator.ApplyOptions();

            var stopwatch = Stopwatch.StartNew();
            var (whitePlayer, blackPlayer) = (referenceParticipant, experimentalParticipant);

            for (var gameIndex = 0; gameIndex < SettingsLoader.Data.GamesPerFitnessTest; gameIndex++)
            {
                try
                {
                    var gameData = new GameData(_polyglotBook.GetRandomOpening(SettingsLoader.Data.PolyglotMaxMoves));
                    var (playerToMove, opponent) = (whitePlayer, blackPlayer);

                    playerToMove.EngineOperator.InitNewGame();
                    opponent.EngineOperator.InitNewGame();

                    while (true)
                    {
                        var bestMoveData = playerToMove.EngineOperator.Go(gameData.HalfMovesDone, gameData.WhiteClock, gameData.BlackClock);
                        if (bestMoveData == null)
                        {
                            playerToMove.History.Add(new ArchivedGame(gameData, GameResult.Draw));
                            opponent.History.Add(new ArchivedGame(gameData, GameResult.Draw));
                            break;
                        }

                        playerToMove.Logs.Add(bestMoveData.LastInfoData);
                        gameData.MakeMove(bestMoveData);

                        if (gameData.GameIsDone)
                        {
                            if (gameData.WhiteClock <= 0 || gameData.BlackClock <= 0)
                            {
                                playerToMove.History.Add(new ArchivedGame(gameData, GameResult.Loss));
                                opponent.History.Add(new ArchivedGame(gameData, GameResult.Win));
                            }
                            else if (gameData.Winner == Color.None)
                            {
                                playerToMove.History.Add(new ArchivedGame(gameData, GameResult.Draw));
                                opponent.History.Add(new ArchivedGame(gameData, GameResult.Draw));
                            }
                            else
                            {
                                playerToMove.History.Add(new ArchivedGame(gameData, GameResult.Win));
                                opponent.History.Add(new ArchivedGame(gameData, GameResult.Loss));
                            }

                            break;
                        }

                        (playerToMove, opponent) = (opponent, playerToMove);
                    }

                    (whitePlayer, blackPlayer) = (blackPlayer, whitePlayer);
                }
                catch
                {
                    referenceParticipant.EngineOperator.Restart();
                    experimentalParticipant.EngineOperator.Restart();
                    gameIndex--;
                }
            }

            var elapsedTime = (double)stopwatch.ElapsedMilliseconds / 1000;
            var fitness = experimentalParticipant.Wins - referenceParticipant.Wins + referenceParticipant.Draws / 2;

            var chromosomeRequest = RequestsFactory.CreateChromosomeRequest(_testId, fitness, elapsedTime, chromosome, referenceParticipant, experimentalParticipant);
            _webService.SendChromosomeData(chromosomeRequest).GetAwaiter().GetResult();

            Console.WriteLine($"[{DateTime.Now}] Run done! Fitness: {fitness}");
            return fitness;
        }
    }
}
