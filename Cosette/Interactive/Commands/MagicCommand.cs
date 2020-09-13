using System;
using System.Linq;
using Cosette.Engine.Moves.Magic;

namespace Cosette.Interactive.Commands
{
    public class MagicCommand : ICommand
    {
        public string Description { get; }
        private InteractiveConsole _interactiveConsole;

        public MagicCommand(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;
            Description = "Generate new magic numbers and display them";
        }

        public void Run(params string[] parameters)
        {
            _interactiveConsole.WriteLine("Generating...");
            var rookAttacks = MagicBitboards.GenerateRookAttacks();
            var bishopAttacks = MagicBitboards.GenerateBishopAttacks();

            _interactiveConsole.WriteLine("Rook magic numbers: ");
            _interactiveConsole.WriteLine(string.Join(",\n", rookAttacks.Select(p => p.MagicNumber)));
            _interactiveConsole.WriteLine();
            _interactiveConsole.WriteLine("Bishop magic numbers: ");
            _interactiveConsole.WriteLine(string.Join(",\n", bishopAttacks.Select(p => p.MagicNumber)));
        }
    }
}