using System;
using Cosette.Engine.Ai.Transposition;

namespace Cosette.Uci.Commands
{
    public class SetOptionCommand : IUciCommand
    {
        private UciClient _uciClient;
        private UciGame _uciGame;
        private const int PawnHashTableSize = 8;

        public SetOptionCommand(UciClient uciClient, UciGame uciGame)
        {
            _uciClient = uciClient;
            _uciGame = uciGame;
        }

        public void Run(params string[] parameters)
        {
            switch (parameters[1])
            {
                case "Hash":
                {
                    TranspositionTable.Init(ulong.Parse(parameters[3]) - PawnHashTableSize);
                    PawnHashTable.Init(PawnHashTableSize);
                    break;
                }
            }
        }
    }
}