using Newtonsoft.Json;

namespace Cosette.Tuner.Settings
{
    public class SettingsModel
    {
        [JsonProperty("engine_path")]
        public int EnginePath { get; set; }

        [JsonProperty("engine_arguments")]
        public int EngineArguments { get; set; }
    }
}
