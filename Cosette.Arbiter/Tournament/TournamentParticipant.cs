using Cosette.Arbiter.Engine;
using Cosette.Arbiter.Settings;

namespace Cosette.Arbiter.Tournament
{
    public class TournamentParticipant
    {
        public EngineData EngineData { get; }
        public EngineOperator EngineOperator { get; }

        public int CurrentRating { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Draws { get; set; }

        public TournamentParticipant(EngineData engineData, EngineOperator engineOperator)
        {
            EngineData = engineData;
            EngineOperator = engineOperator;
            CurrentRating = engineData.Rating;
        }
    }
}
