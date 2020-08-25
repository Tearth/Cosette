using System;
using System.Collections.Generic;
using System.Linq;
using Cosette.Engine.Common;

namespace Cosette.Uci.Commands
{
    public class PositionCommand : IUciCommand
    {
        private UciClient _uciClient;
        private UciGame _uciGame;

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
                    ParseStartPos(settings.ToList());
                    break;
                }

                case "fen":
                {
                    var fen = string.Join(" ", settings.Take(6));
                    var moves = settings.Skip(6).ToList();
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
                var from = Position.FromText(move.Substring(0, 2));
                var to = Position.FromText(move.Substring(2, 2));

                if (!_uciGame.MakeMove(color, from, to))
                {
                    throw new InvalidOperationException();
                }

                color = ColorOperations.Invert(color);
            }
        }
    }
}