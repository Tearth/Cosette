namespace Cosette.Arbiter.Tournament
{
    public class ArchivedGame
    {
        public GameData GameData { get; }
        public TournamentParticipant Opponent { get; }
        public GameResult Result { get; }
        public bool TimeFlag { get; }

        public ArchivedGame(GameData gameData, TournamentParticipant opponent, GameResult result, bool timeFlag = false)
        {
            GameData = gameData;
            Opponent = opponent;
            Result = result;
            TimeFlag = timeFlag;
        }
    }
}
