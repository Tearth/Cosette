using System;
using System.Linq;
using Cosette.Engine.Moves.Magic;

namespace Cosette.Interactive.Commands
{
    public class MagicCommand : ICommand
    {
        public string Description { get; }

        public MagicCommand()
        {
            Description = "Generate new magic numbers and display them";
        }

        public void Run()
        {
            Console.WriteLine("Generating...");
            var rookAttacks = MagicBitboards.GenerateRookAttacks();
            var bishopAttacks = MagicBitboards.GenerateBishopAttacks();

            Console.WriteLine("Rook magic numbers: ");
            Console.WriteLine(string.Join(",\n", rookAttacks.Select(p => p.MagicNumber)));
            Console.WriteLine();
            Console.WriteLine("Bishop magic numbers: ");
            Console.WriteLine(string.Join(",\n", bishopAttacks.Select(p => p.MagicNumber)));
        }
    }
}