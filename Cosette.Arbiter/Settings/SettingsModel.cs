using Newtonsoft.Json;

namespace Cosette.Arbiter.Settings
{
    public class SettingsModel
    {
        [JsonProperty("engine1_name")]
        public string Engine1Name { get; set; }

        [JsonProperty("engine1_path")]
        public string Engine1Path { get; set; }

        [JsonProperty("engine2_name")]
        public string Engine2Name { get; set; }

        [JsonProperty("engine2_path")]
        public string Engine2Path { get; set; }

        [JsonProperty("milliseconds_per_move")]
        public int MillisecondsPerMove { get; set; }
    }
}
