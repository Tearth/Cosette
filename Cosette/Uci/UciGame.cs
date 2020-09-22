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

        public UciGame()
        {
            BoardState = new BoardState();
        }

        public void SetDefaultState()
        {
            BoardState.SetDefaultState();
        }

        public void SetFen(string fen)
        {
            BoardState = FenParser.Parse(fen);
        }

        public void MakeMove(Move move)
        {
            BoardState.MakeMove(move);
        }

        public Move SearchBestMove(SearchContext context)
        {
            SearchContext = context;
            return IterativeDeepening.FindBestMove(context);
        }
    }
}
