using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cosette.Tuner.Texel.Settings
{
    public class SettingsModel
    {
        public List<GeneInfo> Genes { get; set; }

        [JsonProperty("engine_path")]
        public string EnginePath { get; set; }

        [JsonProperty("engine_arguments")]
        public string EngineArguments { get; set; }

        [JsonProperty("positions_database_path")]
        public string PositionsDatabasePath { get; set; }

        [JsonProperty("min_population")]
        public int MinPopulation { get; set; }

        [JsonProperty("max_population")]
        public int MaxPopulation { get; set; }

        [JsonProperty("generations_count")]
        public int GenerationsCount { get; set; }

        [JsonProperty("scaling_constant")]
        public double ScalingConstant { get; set; }
    }
}
