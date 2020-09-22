using System;
using System.Collections.Generic;
using System.Linq;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;

namespace Cosette.Uci.Commands
{
    public class PositionCommand : IUciCommand
    {
        private UciClient _uciClient;
        private readonly UciGame _uciGame;

        public PositionCommand(UciClient uciClient, UciGame uciGame)
        {
            _uciClient = uciClient;
            _uciGame = uciGame;
        }

        public void Run(params string[] parameters)
        {
            var positionType = parameters[0];
            var settings = parameters.Skip(1).ToList();

            switch (positionType)
            {
                case "startpos":
                {
                    var moves = settings.Skip(1).ToList();
                    ParseStartPos(moves);
                    break;
                }

                case "fen":
                {
                    var fen = string.Join(" ", settings.Take(6));
                    var moves = settings.Skip(7).ToList();
                    ParseFen(fen, moves);
                    break;
                }
            }
        }

        private void ParseStartPos(List<string> moves)
        {
            _uciGame.SetDefaultState();
            ParseMoves(moves);
        }

        private void ParseFen(string fen, List<string> moves)
        {
            _uciGame.SetFen(fen);
            ParseMoves(moves);
        }

        private void ParseMoves(List<string> moves)
        {
            var color = Color.White;
            foreach (var move in moves)
            {
                var parsedMove = Move.FromTextNotation(_uciGame.BoardState, move);
                if (parsedMove == Move.Empty)
                {
                    throw new InvalidOperationException();
                }

                _uciGame.MakeMove(parsedMove);
                color = ColorOperations.Invert(color);
            }
        }
    }
}