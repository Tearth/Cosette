using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cosette.Arbiter.Settings
{
    public class SettingsModel
    {
        public List<EngineData> Engines { get; set; }
        public List<string> Options { get; set; }

        [JsonProperty("milliseconds_per_move")]
        public int MillisecondsPerMove { get; set; }

        [JsonProperty("games_count")]
        public int GamesCount { get; set; }
    }
}
