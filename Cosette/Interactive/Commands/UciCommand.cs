using System;
using System.Diagnostics;
using System.Linq;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Moves;
using Cosette.Engine.Moves.Magic;
using Cosette.Engine.Perft;
using Cosette.Uci;

namespace Cosette.Interactive.Commands
{
    public class UciCommand : ICommand
    {
        public string Description { get; }

        public UciCommand()
        {
            Description = "Run UCI client";
        }

        public void Run(params string[] parameters)
        {
            new UciClient().Run();
        }
    }
}