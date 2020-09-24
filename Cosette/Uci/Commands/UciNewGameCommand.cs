using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Transposition;

namespace Cosette.Uci.Commands
{
    public class UciNewGameCommand : IUciCommand
    {
        private readonly UciClient _uciClient;

        public UciNewGameCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            TranspositionTable.Clear();
            PawnHashTable.Clear();
            KillerHeuristic.Clear();
            HistoryHeuristic.Clear();

            _uciClient.BoardState.SetDefaultState();
        }
    }
}