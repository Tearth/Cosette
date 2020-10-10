namespace Cosette.Arbiter.Tournament
{
    public class ArchivedGame
    {
        public GameData GameData { get; }
        public GameResult Result { get; }

        public ArchivedGame(GameData gameData, GameResult result)
        {
            GameData = gameData;
            Result = result;
        }
    }
}
