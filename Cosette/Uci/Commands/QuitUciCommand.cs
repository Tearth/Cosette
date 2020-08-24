﻿using System;

namespace Cosette.Uci.Commands
{
    public class QuitUciCommand : IUciCommand
    {
        private UciClient _uciClient;

        public QuitUciCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
        }

        public void Run(params string[] parameters)
        {
            Environment.Exit(0);
        }
    }
}
