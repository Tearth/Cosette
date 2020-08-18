using System;

namespace Cosette.Interactive.Commands
{
    public class QuitCommand : ICommand
    {
        public void Run()
        {
            Environment.Exit(0);
        }
    }
}
