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

        public int CurrentRating { get; set; }
        public int Wins => History.Count(p => p.Result == GameResult.Win);
        public int Losses => History.Count(p => p.Result == GameResult.Loss);
        public int Draws => History.Count(p => p.Result == GameResult.Draw);

        public TournamentParticipant(EngineData engineData, EngineOperator engineOperator)
        {
            EngineData = engineData;
            EngineOperator = engineOperator;
            History = new List<ArchivedGame>();

            CurrentRating = engineData.Rating;
        }
    }
}
