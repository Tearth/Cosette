namespace Cosette.Uci.Commands
{
    public class DebugCommand : IUciCommand
    {
        private readonly UciClient _uciClient;

        public DebugCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            _uciClient.SetDebugMode(parameters[0] == "on");
        }
    }
}