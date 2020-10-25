using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cosette.Tuner.Settings
{
    public class SettingsModel
    {
        public List<string> Options { get; set; }
        public List<GeneInfo> Genes { get; set; }

        [JsonProperty("engine_path")]
        public string EnginePath { get; set; }

        [JsonProperty("engine_arguments")]
        public string EngineArguments { get; set; }

        [JsonProperty("base_time")]
        public int BaseTime { get; set; }

        [JsonProperty("inc_time")]
        public int IncTime { get; set; }
    }
}
