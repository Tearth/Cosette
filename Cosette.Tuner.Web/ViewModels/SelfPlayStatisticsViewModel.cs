using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cosette.Tuner.Web.ViewModels
{
    public class SelfPlayStatisticsViewModel
    {
        public int Id { get; set; }

        public DateTime CreationTimeUtc { get; set; }
        public bool IsReferenceEngine { get; set; }
        public double AverageTimePerGame { get; set; }
        public double AverageDepth { get; set; }
        public double AverageNodesCount { get; set; }
        public double AverageNodesPerSecond { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
    }
}
