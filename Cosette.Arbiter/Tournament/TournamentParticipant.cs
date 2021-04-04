using System.Collections.Generic;
using System.Linq;
using Cosette.Arbiter.Engine;
using Cosette.Arbiter.Settings;

namespace Cosette.Arbiter.Tournament
{
    public class TournamentParticipant
    {
        public EngineData EngineData { get; }
        public EngineOperator EngineOperator { get; }
        public List<ArchivedGame> History { get; }
        public List<InfoData> Logs { get; }

        public int Wins => History.Count(p => p.Result == GameResult.Win);
        public int Losses => History.Count(p => p.Result == GameResult.Loss);
        public int Draws => History.Count(p => p.Result == GameResult.Draw);
        public double AverageDepth => Logs.Count == 0 ? 0 : Logs.Average(p => p.Depth);
        public int AverageNodesCount => Logs.Count == 0 ? 0 : (int)Logs.Average(p => (int)p.Nodes);
        public int AverageNps => Logs.Count == 0 ? 0 : (int)Logs.Average(p => (int)p.Nps);

        public TournamentParticipant(EngineData engineData, EngineOperator engineOperator)
        {
            EngineData = engineData;
            EngineOperator = engineOperator;
            History = new List<ArchivedGame>();
            Logs = new List<InfoData>();
        }

        public int CalculatePerformanceRating()
        {
            if (History.Count == 0)
            {
                return 0;
            }

            return 400 * (Wins - Losses) / History.Count;
        }

        public int WonGamesPercent()
        {
            if (History.Count == 0 || History.Count == Draws)
            {
                return 0;
            }

            return (int)((float)Wins * 100 / (Wins + Losses));
        }
    }
}
