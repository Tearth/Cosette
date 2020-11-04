using System;
using System.Collections.Generic;
using System.Diagnostics;
using Cosette.Polyglot;
using Cosette.Tuner.Engine;
using Cosette.Tuner.Genetics.Game;
using Cosette.Tuner.Settings;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;

namespace Cosette.Tuner.Genetics
{
    public class EvaluationFitness : IFitness
    {
        private EngineOperator _referenceEngineOperator;
        private EngineOperator _experimentalEngineOperator;
        private PolyglotBook _polyglotBook;

        public EvaluationFitness()
        {
            _referenceEngineOperator = new EngineOperator(SettingsLoader.Data.EnginePath, SettingsLoader.Data.EngineArguments);
            _experimentalEngineOperator = new EngineOperator(SettingsLoader.Data.EnginePath, SettingsLoader.Data.EngineArguments);
            _polyglotBook = new PolyglotBook(SettingsLoader.Data.PolyglotOpeningBook);

            _referenceEngineOperator.Init();
            _experimentalEngineOperator.Init();
        }

        public double Evaluate(IChromosome chromosome)
        {
            var referenceEngineWins = 0;
            var experimentalEngineWins = 0;
            var draws = 0;

            for (var geneIndex = 0; geneIndex < SettingsLoader.Data.Genes.Count; geneIndex++)
            {
                _experimentalEngineOperator.SetOption(SettingsLoader.Data.Genes[geneIndex].Name, chromosome.GetGene(geneIndex).ToString());
            }

            _referenceEngineOperator.ApplyOptions();
            _experimentalEngineOperator.ApplyOptions();

            var stopwatch = Stopwatch.StartNew();
            for (var gameIndex = 0; gameIndex < SettingsLoader.Data.GamesPerFitnessTest; gameIndex++)
            {
                try
                {
                    var gameData = new GameData(_polyglotBook.GetRandomOpening(SettingsLoader.Data.PolyglotMaxMoves));

                    var whitePlayer = DateTime.UtcNow.Ticks % 2 == 0 ? _referenceEngineOperator : _experimentalEngineOperator;
                    var blackPlayer = whitePlayer == _referenceEngineOperator ? _experimentalEngineOperator : _referenceEngineOperator;
                    var (playerToMove, opponent) = (whitePlayer, blackPlayer);

                    whitePlayer.InitNewGame();
                    blackPlayer.InitNewGame();

                    while (true)
                    {
                        var bestMoveData = playerToMove.Go(gameData.HalfMovesDone, gameData.WhiteClock, gameData.BlackClock);
                        if (bestMoveData == null)
                        {
                            draws++;
                            break;
                        }

                        gameData.MakeMove(bestMoveData);

                        if (gameData.GameIsDone)
                        {
                            if (gameData.WhiteClock <= 0 || gameData.BlackClock <= 0)
                            {
                                if (playerToMove == _referenceEngineOperator)
                                {
                                    experimentalEngineWins++;
                                }
                                else
                                {
                                    referenceEngineWins++;
                                }
                            }
                            else if (gameData.Winner == Color.None)
                            {
                                draws++;
                            }
                            else
                            {
                                if (playerToMove == _referenceEngineOperator)
                                {
                                    referenceEngineWins++;
                                }
                                else
                                {
                                    experimentalEngineWins++;
                                }
                            }

                            break;
                        }

                        (playerToMove, opponent) = (opponent, playerToMove);
                    }
                }
                catch
                {
                    _referenceEngineOperator.Restart();
                    _experimentalEngineOperator.Restart();
                    gameIndex--;
                }
            }

            var runTime = stopwatch.ElapsedMilliseconds;
            var averageTimePerGame = runTime / SettingsLoader.Data.GamesPerFitnessTest;
            var fitness = experimentalEngineWins - referenceEngineWins + draws / 2;

            var genesList = new List<string>();
            for (var geneIndex = 0; geneIndex < SettingsLoader.Data.Genes.Count; geneIndex++)
            {
                var name = SettingsLoader.Data.Genes[geneIndex].Name;
                var value = chromosome.GetGene(geneIndex).ToString();

                genesList.Add($"{name}={value}");
            }

            Console.WriteLine($"[{DateTime.Now}] Run done!");
            Console.WriteLine($" - reference wins: {referenceEngineWins}, experimental wins: {experimentalEngineWins}, draws: {draws}");
            Console.WriteLine($" - fitness: {fitness}, {string.Join(", ", genesList)}");
            Console.WriteLine($" - average time per game: {(double)averageTimePerGame / 1000} s");

            return fitness;
        }
    }
}
