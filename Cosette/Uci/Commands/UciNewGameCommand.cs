namespace Cosette.Uci.Commands
{
    public class UciNewGameCommand : IUciCommand
    {
        private UciClient _uciClient;
        private readonly UciGame _uciGame;

        public UciNewGameCommand(UciClient uciClient, UciGame uciGame)
        {
            _uciClient = uciClient;
            _uciGame = uciGame;
        }

        public void Run(params string[] parameters)
        {
            _uciGame.SetDefaultState();
        }
    }
}