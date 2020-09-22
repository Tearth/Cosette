namespace Cosette.Uci.Commands
{
    public class IsReadyCommand : IUciCommand
    {
        private readonly UciClient _uciClient;

        public IsReadyCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            _uciClient.Send("readyok");
        }
    }
}