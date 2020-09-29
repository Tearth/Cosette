using System;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Board;
using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai.Search
{
    public class SearchStatistics
    {
        public BoardState Board { get; set; }
        public EvaluationStatistics EvaluationStatistics { get; set; }

        public int Depth { get; set; }
        public int SelectiveDepth { get; set; }
        public int Score { get; set; }
        public ulong SearchTime { get; set; }

        public ulong Nodes { get; set; }
        public ulong QNodes { get; set; }
        public ulong TotalNodes => Nodes + QNodes;

        public ulong Leafs { get; set; }
        public ulong QLeafs { get; set; }
        public ulong TotalLeafs => Leafs + QLeafs;

        public ulong TotalNodesPerSecond => (ulong)(TotalNodes / ((float) SearchTime / 1000));
        public float SecondsPerNode => (float) SearchTime / 1000 / Nodes;

        public float BranchingFactor => (float) Nodes / (Nodes - Leafs);
        public float QBranchingFactor => (float) QNodes / (QNodes - QLeafs);
        public float TotalBranchingFactor => (float) TotalNodes / (TotalNodes - TotalLeafs);

        public ulong BetaCutoffs { get; set; }
        public ulong QBetaCutoffs { get; set; }
        public ulong TotalBetaCutoffs => BetaCutoffs + QBetaCutoffs;

        public ulong TTEntries { get; set; }
        public ulong TTCollisions { get; set; }
        public ulong TTHits { get; set; }
        public ulong TTNonHits { get; set; }
        public float TTHitsPercent => (float) TTHits * 100 / (TTHits + TTNonHits);

        public int BetaCutoffsAtFirstMove { get; set; }
        public int QBetaCutoffsAtFirstMove { get; set; }
        public int TotalBetaCutoffsAtFirstMove => BetaCutoffsAtFirstMove + QBetaCutoffsAtFirstMove;
        
        public int BetaCutoffsNotAtFirstMove { get; set; }
        public int QBetaCutoffsNotAtFirstMove { get; set; }
        public int TotalBetaCutoffsAtNotFirstMove => BetaCutoffsNotAtFirstMove + QBetaCutoffsNotAtFirstMove;

        public float BetaCutoffsAtFirstMovePercent => (float) BetaCutoffsAtFirstMove * 100 / (BetaCutoffsAtFirstMove + BetaCutoffsNotAtFirstMove);
        public float QBetaCutoffsAtFirstMovePercent => (float) QBetaCutoffsAtFirstMove * 100 / (QBetaCutoffsAtFirstMove + QBetaCutoffsNotAtFirstMove);

        public Move[] PrincipalVariation { get; set; }
        public int PrincipalVariationMovesCount { get; set; }

        public SearchStatistics()
        {
            EvaluationStatistics = new EvaluationStatistics();
            PrincipalVariation = new Move[SearchConstants.MaxDepth];
        }

        public void Clear()
        {
            Board = null;
            EvaluationStatistics.Clear();

            Depth = 0;
            SelectiveDepth = 0;
            Score = 0;
            SearchTime = 0;

            Nodes = 0;
            QNodes = 0;

            Leafs = 0;
            QLeafs = 0;

            BetaCutoffs = 0;
            QBetaCutoffs = 0;

            TTEntries = 0;
            TTCollisions = 0;
            TTHits = 0;

            BetaCutoffsAtFirstMove = 0;
            QBetaCutoffsAtFirstMove = 0;

            Array.Clear(PrincipalVariation, 0, PrincipalVariation.Length);
            PrincipalVariationMovesCount = 0;
        }
    }
}
