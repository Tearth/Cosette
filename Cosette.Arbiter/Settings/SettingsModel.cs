using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cosette.Arbiter.Settings
{
    public class SettingsModel
    {
        public List<EngineData> Engines { get; set; }
        public List<string> Options { get; set; }

        [JsonProperty("base_time")]
        public int BaseTime { get; set; }

        [JsonProperty("inc_time")]
        public int IncTime { get; set; }

        [JsonProperty("polyglot_opening_book")]
        public string PolyglotOpeningBook { get; set; }

        [JsonProperty("polyglot_max_moves")]
        public int PolyglotMaxMoves { get; set; }

        [JsonProperty("max_moves_count")]
        public int MaxMovesCount { get; set; }

        [JsonProperty("games_count")]
        public int GamesCount { get; set; }

        [JsonProperty("gauntlet")]
        public bool Gauntlet { get; set; }
    }
}
