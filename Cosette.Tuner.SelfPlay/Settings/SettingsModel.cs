using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cosette.Tuner.SelfPlay.Settings
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

        [JsonProperty("polyglot_opening_book")]
        public string PolyglotOpeningBook { get; set; }

        [JsonProperty("polyglot_max_moves")]
        public int PolyglotMaxMoves { get; set; }

        [JsonProperty("max_moves_count")]
        public int MaxMovesCount { get; set; }

        [JsonProperty("min_population")]
        public int MinPopulation { get; set; }

        [JsonProperty("max_population")]
        public int MaxPopulation { get; set; }

        [JsonProperty("generations_count")]
        public int GenerationsCount { get; set; }

        [JsonProperty("games_per_fitness_test")]
        public int GamesPerFitnessTest { get; set; }
    }
}
