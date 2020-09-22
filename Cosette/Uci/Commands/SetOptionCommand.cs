using Cosette.Engine.Ai.Search;
using Cosette.Engine.Ai.Transposition;

namespace Cosette.Uci.Commands
{
    public class SetOptionCommand : IUciCommand
    {
        private readonly UciClient _uciClient;

        public SetOptionCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            switch (parameters[1])
            {
                case "Hash":
                {
                    TranspositionTable.Init(ulong.Parse(parameters[3]) - SearchConstants.DefaultPawnHashTableSize);
                    PawnHashTable.Init(SearchConstants.DefaultPawnHashTableSize);
                    break;
                }
            }
        }
    }
}