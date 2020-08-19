using System;
using System.Linq;
using Cosette.Engine.Board;
using Cosette.Engine.Moves;
using Cosette.Engine.Moves.Magic;

namespace Cosette.Interactive.Commands
{
    public class PerftCommand : ICommand
    {
        public string Description { get; }

        public PerftCommand()
        {
            Description = "Test performance of the moves generator";
        }

        public unsafe void Run()
        {
            var boardState = new BoardState();
            boardState.SetDefaultState();

            Span<Move> moves = stackalloc Move[128];
            boardState.GetAvailableMoves(moves);
        }
    }
}