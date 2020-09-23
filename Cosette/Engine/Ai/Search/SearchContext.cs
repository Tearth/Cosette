using System.Collections.Generic;
using Cosette.Engine.Board;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Search
{
    public class SearchContext
    {
        public BoardState BoardState { get; set; }
        public SearchStatistics Statistics { get; set; }

        public int MaxDepth { get; set; }
        public int MaxTime { get; set; }
        public bool AbortSearch { get; set; }
        public bool WaitForStopCommand { get; set; }
        public ulong MaxNodesCount { get; set; }
        public List<Move> MoveRestrictions { get; set; }
        public int TranspositionTableEntryAge { get; set; }

        public SearchContext(BoardState boardState)
        {
            BoardState = boardState;
            Statistics = new SearchStatistics();

            MaxDepth = SearchConstants.MaxDepth;
            MaxTime = int.MaxValue;
            WaitForStopCommand = false;
            MaxNodesCount = ulong.MaxValue;
            MoveRestrictions = null;
            TranspositionTableEntryAge = boardState.MovesCount;
        }
    }
}
