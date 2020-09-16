using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosette.Engine.Ai;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Score.Evaluators;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Interactive;
using Cosette.Logs;
using Cosette.Uci.Commands;

namespace Cosette.Uci
{
    public class UciClient
    {
        private UciGame _uciGame;
        private bool _debugMode;

        private InteractiveConsole _interactiveConsole;
        private Dictionary<string, IUciCommand> _commands;

        public UciClient(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;

            _uciGame = new UciGame();

#if UCI_DEBUG_OUTPUT
            _debugMode = true;
#endif

            _commands = new Dictionary<string, IUciCommand>();
            _commands["quit"] = new QuitCommand(this, _uciGame);
            _commands["setoption"] = new SetOptionCommand(this, _uciGame);
            _commands["isready"] = new IsReadyCommand(this, _uciGame);
            _commands["ucinewgame"] = new UciNewGameCommand(this, _uciGame);
            _commands["position"] = new PositionCommand(this, _uciGame);
            _commands["debug"] = new DebugCommand(this, _uciGame);
            _commands["go"] = new GoCommand(this, _uciGame);

            IterativeDeepening.OnSearchUpdate += OnSearchUpdate;
        }

        public void Run()
        {
            SendName();
            SendAuthor();
            SendOptions();
            RunCommandsLoop();
        }

        public void Send(string command)
        {
            _interactiveConsole.WriteLine(command);
            LogManager.LogInfo("[SEND] " + command);
        }

        public (string Command, string[] parameters) Receive()
        {
            var input = Console.ReadLine();
            var splitInput = input.Split(' ');
            var command = splitInput[0].ToLower();
            var parameters = splitInput.Skip(1).ToArray();

            LogManager.LogInfo("[RECV] " + input);
            return (command, parameters);
        }

        public void SetDebugMode(bool state)
        {
            _debugMode = state;
        }

        private void SendName()
        {
            Send("id name Cosette");
        }

        private void SendAuthor()
        {
            Send("id author Tearth");
        }

        private void SendOptions()
        {
            var defaultHashTablesSize = SearchConstants.DefaultHashTableSize + SearchConstants.DefaultPawnHashTableSize;

            Send($"option name Hash type spin default {defaultHashTablesSize} min 1 max 2048");
            Send("uciok");
        }

        private void RunCommandsLoop()
        {
            while (true)
            {
                var (command, parameters) = Receive();
                if (_commands.ContainsKey(command))
                {
                    _commands[command].Run(parameters);
                }
            }
        }

        private void OnSearchUpdate(object sender, SearchStatistics stats)
        {
            var score = FormatScore(stats.Score);
            var principalVariation = FormatPrincipalVariation(stats.PrincipalVariation, stats.PrincipalVariationMovesCount);

            Send($"info depth {stats.Depth} time {stats.SearchTime} pv {principalVariation} score {score} nodes {stats.TotalNodes} nps {stats.TotalNodesPerSecond}");

            if (_debugMode)
            {
                var openingPhase = stats.Board.GetPhaseRatio();
                var endingPhase = 1 - openingPhase;

                var materialEvaluation = MaterialEvaluator.Evaluate(stats.Board, openingPhase, endingPhase);
                var castlingEvaluation = CastlingEvaluator.Evaluate(stats.Board, openingPhase, endingPhase);
                var positionEvaluation = PositionEvaluator.Evaluate(stats.Board, openingPhase, endingPhase);
                var pawnStructureEvaluation = PawnStructureEvaluator.Evaluate(stats.Board, openingPhase, endingPhase);
                var mobility = MobilityEvaluator.Evaluate(stats.Board, openingPhase, endingPhase);
                var kingSafety = KingSafetyEvaluator.Evaluate(stats.Board, openingPhase, endingPhase);
                var total = materialEvaluation + castlingEvaluation + positionEvaluation + pawnStructureEvaluation + mobility + kingSafety;

                Send($"info string evaluation {total} phase {openingPhase:F} material {materialEvaluation} castling {castlingEvaluation} " +
                     $"position {positionEvaluation} pawns {pawnStructureEvaluation} mobility {mobility} ksafety {kingSafety}");
            }
        }

        private string FormatScore(int score)
        {
            if (IterativeDeepening.IsScoreCheckmate(score))
            {
                var movesToCheckmate = IterativeDeepening.GetMovesToCheckmate(score);
                if (score < 0)
                {
                    movesToCheckmate = -movesToCheckmate;
                }

                return "mate " + movesToCheckmate;
            }

            return "cp " + score;
        }

        private string FormatPrincipalVariation(Move[] moves, int movesCount)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < movesCount; i++)
            {
                stringBuilder.Append(moves[i]);
                stringBuilder.Append(' ');
            }

            return stringBuilder.ToString().Trim();
        }
    }
}
