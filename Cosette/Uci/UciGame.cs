using Cosette.Engine.Board;

namespace Cosette.Uci
{
    public class UciGame
    {
        private BoardState _boardState;

        public UciGame()
        {
            _boardState = new BoardState();
        }

        public void SetDefaultState()
        {
            _boardState.SetDefaultState();
        }
    }
}
