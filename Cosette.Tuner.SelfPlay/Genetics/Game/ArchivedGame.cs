namespace Cosette.Tuner.SelfPlay.Genetics.Game
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
