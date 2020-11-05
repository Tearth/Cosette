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
        private string _testName;
        private WebService _webService;
        private EvaluationParticipant _referenceParticipant;
        private EvaluationParticipant _experimentalParticipant;
        private PolyglotBook _polyglotBook;

        public EvaluationFitness(string testName, WebService webService)
        {
            _testName = testName;
            _webService = webService;
            _referenceParticipant = new EvaluationParticipant(new EngineOperator(SettingsLoader.Data.EnginePath, SettingsLoader.Data.EngineArguments));
            _experimentalParticipant = new EvaluationParticipant(new EngineOperator(SettingsLoader.Data.EnginePath, SettingsLoader.Data.EngineArguments));
            _polyglotBook = new PolyglotBook(SettingsLoader.Data.PolyglotOpeningBook);

            _referenceParticipant.EngineOperator.Init();
            _experimentalParticipant.EngineOperator.Init();
        }

        public double Evaluate(IChromosome chromosome)
        {
            for (var geneIndex = 0; geneIndex < SettingsLoader.Data.Genes.Count; geneIndex++)
            {
                _experimentalParticipant.EngineOperator.SetOption(SettingsLoader.Data.Genes[geneIndex].Name, chromosome.GetGene(geneIndex).ToString());
            }

            _referenceParticipant.EngineOperator.ApplyOptions();
            _experimentalParticipant.EngineOperator.ApplyOptions();

            var stopwatch = Stopwatch.StartNew();
            var (whitePlayer, blackPlayer) = (_referenceParticipant, _experimentalParticipant);
            var winsByTime = 0;

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
                                winsByTime++;
                            }
                            
                            if (gameData.Winner == Color.None)
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
                    _referenceParticipant.EngineOperator.Restart();
                    _experimentalParticipant.EngineOperator.Restart();
                    gameIndex--;
                }
            }

            var elapsedTime = (double)stopwatch.ElapsedMilliseconds / 1000;
            var fitness = _experimentalParticipant.Wins - _referenceParticipant.Wins + _referenceParticipant.Draws / 2;

            var chromosomeRequest = GenerateChromosomeRequest(fitness, elapsedTime, chromosome, _referenceParticipant, _experimentalParticipant);
            _webService.SendChromosomeData(chromosomeRequest).GetAwaiter().GetResult();

            Console.WriteLine($"[{DateTime.Now}] Run done! Fitness: {fitness}");
            return fitness;
        }

        private ChromosomeDataRequest GenerateChromosomeRequest(int fitness, double elapsedTime, IChromosome chromosome, EvaluationParticipant referenceParticipant, EvaluationParticipant experimentalParticipant)
        {
            var genes = new List<GeneDataRequest>();
            for (var geneIndex = 0; geneIndex < SettingsLoader.Data.Genes.Count; geneIndex++)
            {
                genes.Add(new GeneDataRequest
                {
                    Name = SettingsLoader.Data.Genes[geneIndex].Name,
                    Value = (int)chromosome.GetGene(geneIndex).Value
                });
            }

            return new ChromosomeDataRequest
            {
                TestName = _testName,
                ElapsedTime = elapsedTime,
                Fitness = fitness,
                ReferenceEngineWins = referenceParticipant.Wins,
                ExperimentalEngineWins = experimentalParticipant.Wins,
                Draws = referenceParticipant.Draws,

                ReferenceEngineStatistics = new EngineStatisticsDataRequest
                {
                    AverageTimePerGame = elapsedTime / SettingsLoader.Data.GamesPerFitnessTest,
                    AverageDepth = referenceParticipant.AverageDepth,
                    AverageNodesCount = referenceParticipant.AverageNodesCount,
                    AverageNodesPerSecond = referenceParticipant.AverageNps
                },

                ExperimentalEngineStatistics = new EngineStatisticsDataRequest
                {
                    AverageTimePerGame = elapsedTime / SettingsLoader.Data.GamesPerFitnessTest,
                    AverageDepth = experimentalParticipant.AverageDepth,
                    AverageNodesCount = experimentalParticipant.AverageNodesCount,
                    AverageNodesPerSecond = experimentalParticipant.AverageNps
                },

                Genes = genes
            };
        }
    }
}
