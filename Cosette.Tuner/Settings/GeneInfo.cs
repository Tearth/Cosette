using Newtonsoft.Json;

namespace Cosette.Tuner.Settings
{
    public class GeneInfo
    {
        public string Name { get; set; }

        [JsonProperty("min_value")]
        public int MinValue { get; set; }

        [JsonProperty("max_value")]
        public int MaxValue { get; set; }
    }
}
