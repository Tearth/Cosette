namespace Cosette.Uci.Commands
{
    public class IsReadyCommand : IUciCommand
    {
        private readonly UciClient _uciClient;
        private UciGame _uciGame;

        public IsReadyCommand(UciClient uciClient, UciGame uciGame)
        {
            _uciClient = uciClient;
            _uciGame = uciGame;
        }

        public void Run(params string[] parameters)
        {
            _uciClient.Send("readyok");
        }
    }
}