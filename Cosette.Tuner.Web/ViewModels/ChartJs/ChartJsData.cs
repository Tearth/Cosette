using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Cosette.Tuner.Web.ViewModels.ChartJs
{
    public class ChartJsData<T>
    {
        [JsonProperty("labels")]
        public List<string> Labels { get; set; }

        [JsonProperty("datasets")]
        public List<ChartJsDataset<T>> Datasets { get; set; }
    }
}
