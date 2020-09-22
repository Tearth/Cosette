using Cosette.Engine.Ai.Search;
using Cosette.Engine.Ai.Time;
using Cosette.Engine.Board;
using Cosette.Engine.Common;
using Cosette.Engine.Fen;
using Cosette.Engine.Moves;

namespace Cosette.Uci
{
    public class UciGame
    {
        public BoardState BoardState;
        public SearchContext SearchContext;
        public int CurrentColor;
        public int CurrentMoveNumber;

        public UciGame()
        {
            BoardState = new BoardState();
        }

        public void SetDefaultState()
        {
            BoardState.SetDefaultState();
            CurrentColor = Color.White;
            CurrentMoveNumber = 1;
        }

        public void SetFen(string fen)
        {
            BoardState = FenParser.Parse(fen, out CurrentMoveNumber);
        }

        public void MakeMove(Move move)
        {
            BoardState.MakeMove(move);
            if (BoardState.ColorToMove == Color.White)
            {
                CurrentMoveNumber++;
            }
        }

        public Move SearchBestMove(SearchContext context)
        {
            SearchContext = context;
            return IterativeDeepening.FindBestMove(context);
        }

        public void SetCurrentColor(int color)
        {
            CurrentColor = color;
        }
    }
}
