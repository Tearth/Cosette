using System;
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
            var errors = 0;

            for (var geneIndex = 0; geneIndex < SettingsLoader.Data.Genes.Count; geneIndex++)
            {
                _experimentalEngineOperator.SetOption(SettingsLoader.Data.Genes[geneIndex].Name, chromosome.GetGene(geneIndex).ToString());
            }

            for (var gameIndex = 0; gameIndex < SettingsLoader.Data.GamesPerFitnessTest; gameIndex++)
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
                        errors++;
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

            return experimentalEngineWins - referenceEngineWins;
        }
    }
}
