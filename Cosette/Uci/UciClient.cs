using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Score.Evaluators;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Interactive;
using Cosette.Logs;
using Cosette.Uci.Commands;

namespace Cosette.Uci
{
    public class UciClient
    {
        public BoardState BoardState;
        public SearchContext SearchContext;

        private bool _debugMode;

        private readonly InteractiveConsole _interactiveConsole;
        private readonly Dictionary<string, IUciCommand> _commands;

        public UciClient(InteractiveConsole interactiveConsole)
        {
            BoardState = new BoardState();
            BoardState.SetDefaultState();

            _interactiveConsole = interactiveConsole;

#if UCI_DEBUG_OUTPUT
            _debugMode = true;
#endif

            _commands = new Dictionary<string, IUciCommand>();
            _commands["quit"] = new QuitCommand(this);
            _commands["setoption"] = new SetOptionCommand(this);
            _commands["isready"] = new IsReadyCommand(this);
            _commands["ucinewgame"] = new UciNewGameCommand(this);
            _commands["position"] = new PositionCommand(this);
            _commands["debug"] = new DebugCommand(this);
            _commands["go"] = new GoCommand(this);
            _commands["stop"] = new StopCommand(this);

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
            var defaultHashTablesSize = SearchConstants.DefaultHashTableSize + 
                                        SearchConstants.DefaultPawnHashTableSize +
                                        SearchConstants.DefaultEvaluationHashTableSize;

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

            Send($"info depth {stats.Depth} seldepth {stats.SelectiveDepth} time {stats.SearchTime} " +
                 $"score {score} nodes {stats.TotalNodes} nps {stats.TotalNodesPerSecond} pv {principalVariation}");

            if (_debugMode)
            {
                var evaluationStatistics = new EvaluationStatistics();
                var openingPhase = stats.Board.GetPhaseRatio();
                var endingPhase = 1 - openingPhase;

                var materialEvaluation = MaterialEvaluator.Evaluate(stats.Board);
                var castlingEvaluation = CastlingEvaluator.Evaluate(stats.Board, openingPhase, endingPhase);
                var positionEvaluation = PositionEvaluator.Evaluate(stats.Board, openingPhase, endingPhase);
                var pawnStructureEvaluation = PawnStructureEvaluator.Evaluate(stats.Board, evaluationStatistics, openingPhase, endingPhase);
                var mobility = MobilityEvaluator.Evaluate(stats.Board, openingPhase, endingPhase);
                var kingSafety = KingSafetyEvaluator.Evaluate(stats.Board, openingPhase, endingPhase);
                var pieces = PiecesEvaluator.Evaluate(stats.Board, openingPhase, endingPhase);

                var total = materialEvaluation + castlingEvaluation + positionEvaluation + pawnStructureEvaluation + 
                            mobility + kingSafety + pieces;

                Send($"info string evaluation {total} phase {openingPhase:F} material {materialEvaluation} castling {castlingEvaluation} " +
                     $"position {positionEvaluation} pawns {pawnStructureEvaluation} mobility {mobility} ksafety {kingSafety} " +
                     $"pieces {pieces} irrmoves {stats.Board.IrreversibleMovesCount}");
            }
        }

        private string FormatScore(int score)
        {
            if (IterativeDeepening.IsScoreNearCheckmate(score))
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
