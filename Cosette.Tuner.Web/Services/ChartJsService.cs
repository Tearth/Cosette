using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cosette.Tuner.Web.ViewModels.ChartJs;

namespace Cosette.Tuner.Web.Services
{
    public class ChartJsService
    {
        public ChartJsData<T> GenerateData<T>(List<string> labels, List<ChartJsDataset<T>> datasets)
        {
            return new ChartJsData<T>
            {
                Labels = labels,
                Datasets = datasets
            };
        }

        public ChartJsDataset<T> GenerateDataset<T>(string label, List<T> values, string backgroundColor, string borderColor)
        {
            return new ChartJsDataset<T>
            {
                Label = label,
                Data = values,
                BackgroundColor = backgroundColor,
                BorderColor = borderColor
            };
        }
    }
}
