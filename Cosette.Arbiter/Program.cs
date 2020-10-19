using Cosette.Arbiter.Settings;
using Cosette.Arbiter.Tournament;

namespace Cosette.Arbiter
{
    class Program
    {
        static void Main(string[] args)
        {
            SettingsLoader.Init("settings.json");
            new TournamentArbiter().Run();
        }
    }
}
