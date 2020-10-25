using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cosette.Tuner.Settings
{
    public class SettingsModel
    {
        public List<string> Options { get; set; }

        [JsonProperty("engine_path")]
        public int EnginePath { get; set; }

        [JsonProperty("engine_arguments")]
        public int EngineArguments { get; set; }

        [JsonProperty("base_time")]
        public int BaseTime { get; set; }

        [JsonProperty("inc_time")]
        public int IncTime { get; set; }
    }
}
