using System;

namespace Cosette.Uci.Commands
{
    public class QuitUciCommand : IUciCommand
    {
        public void Run(params string[] parameters)
        {
            Environment.Exit(0);
        }
    }
}
