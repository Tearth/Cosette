using System.Collections.Generic;
using System.Linq;
using Cosette.Tuner.Engine;

namespace Cosette.Tuner.Genetics.Game
{
    public class EvaluationParticipant
    {
        public EngineOperator EngineOperator { get; }
        public List<ArchivedGame> History { get; }
        public List<InfoData> Logs { get; }

        public int Wins => History.Count(p => p.Result == GameResult.Win);
        public int Losses => History.Count(p => p.Result == GameResult.Loss);
        public int Draws => History.Count(p => p.Result == GameResult.Draw);
        public double AverageDepth => Logs.Count == 0 ? 0 : Logs.Average(p => p.Depth);
        public int AverageNodesCount => Logs.Count == 0 ? 0 : (int)Logs.Average(p => (int)p.Nodes);
        public int AverageNps => Logs.Count == 0 ? 0 : (int)Logs.Average(p => (int)p.Nps);

        public EvaluationParticipant(EngineOperator engineOperator)
        {
            EngineOperator = engineOperator;
            History = new List<ArchivedGame>();
            Logs = new List<InfoData>();
        }
    }
}
