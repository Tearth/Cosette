using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cosette.Tuner.Web.ViewModels.ChartJs
{
    public class ChartJsDataset<T>
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("backgroundColor")]
        public string BackgroundColor { get; set; }

        [JsonProperty("borderColor")]
        public string BorderColor { get; set; }

        [JsonProperty("fill")]
        public string Fill { get; set; }

        [JsonProperty("data")]
        public List<T> Data { get; set; }
    }
}
