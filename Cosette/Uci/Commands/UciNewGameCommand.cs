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
            _uciClient.BoardState.SetDefaultState();
        }
    }
}