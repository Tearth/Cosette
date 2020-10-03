using System;
using Cosette.Arbiter.Engine;
using Cosette.Arbiter.Settings;

namespace Cosette.Arbiter.Tournament
{
    public class TournamentArbiter
    {
        private EngineOperator _engine1 { get; set; }
        private EngineOperator _engine2 { get; set; }

        public TournamentArbiter()
        {
            _engine1 = new EngineOperator(SettingsLoader.Data.Engine1Name, SettingsLoader.Data.Engine1Path);
            _engine2 = new EngineOperator(SettingsLoader.Data.Engine2Name, SettingsLoader.Data.Engine2Path);
        }

        public void Run()
        {
            _engine1.Init();
            _engine2.Init();

            Console.ReadLine();
        }
    }
}
