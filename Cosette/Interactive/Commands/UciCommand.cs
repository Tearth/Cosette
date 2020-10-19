using Cosette.Uci;

namespace Cosette.Interactive.Commands
{
    public class UciCommand : ICommand
    {
        public string Description { get; }
        private readonly InteractiveConsole _interactiveConsole;

        public UciCommand(InteractiveConsole interactiveConsole)
        {
            _interactiveConsole = interactiveConsole;
            Description = "Run UCI client";
        }

        public void Run(params string[] parameters)
        {
            new UciClient(_interactiveConsole).Run();
        }
    }
}