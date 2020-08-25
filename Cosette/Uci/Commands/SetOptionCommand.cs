﻿using System;

namespace Cosette.Uci.Commands
{
    public class SetOptionCommand : IUciCommand
    {
        private UciClient _uciClient;
        private UciGame _uciGame;

        public SetOptionCommand(UciClient uciClient, UciGame uciGame)
        {
            _uciClient = uciClient;
            _uciGame = uciGame;
        }

        public void Run(params string[] parameters)
        {
            // Ignore options for now, engine doesn't support any
        }
    }
}