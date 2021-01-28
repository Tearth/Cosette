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

        public ulong TTAddedEntries { get; set; }
        public ulong TTReplacements { get; set; }
        public ulong TTHits { get; set; }
        public ulong TTNonHits { get; set; }
        public ulong TTInvalidMoves { get; set; }
        public ulong TTValidMoves { get; set; }
        public float TTHitsPercent => (float) TTHits * 100 / (TTHits + TTNonHits);
        public float TTReplacesPercent => (float) TTReplacements * 100 / TTAddedEntries;

        public int BetaCutoffsAtFirstMove { get; set; }
        public int QBetaCutoffsAtFirstMove { get; set; }
        public int TotalBetaCutoffsAtFirstMove => BetaCutoffsAtFirstMove + QBetaCutoffsAtFirstMove;
        
        public int BetaCutoffsNotAtFirstMove { get; set; }
        public int QBetaCutoffsNotAtFirstMove { get; set; }
        public int TotalBetaCutoffsAtNotFirstMove => BetaCutoffsNotAtFirstMove + QBetaCutoffsNotAtFirstMove;

        public float BetaCutoffsAtFirstMovePercent => (float) BetaCutoffsAtFirstMove * 100 / (BetaCutoffsAtFirstMove + BetaCutoffsNotAtFirstMove);
        public float QBetaCutoffsAtFirstMovePercent => (float) QBetaCutoffsAtFirstMove * 100 / (QBetaCutoffsAtFirstMove + QBetaCutoffsNotAtFirstMove);

        public int IIDHits { get; set; }
        public int LoudMovesGenerated { get; set; }
        public int QuietMovesGenerated { get; set; }
        public int Extensions { get; set; }
        public int NullMovePrunes { get; set; }
        public int StaticNullMovePrunes { get; set; }
        public int FutilityPrunes { get; set; }
        public int QSEEPrunes { get; set; }
        public int QFutilityPrunes { get; set; }

        public Move[] PrincipalVariation { get; set; }
        public int PrincipalVariationMovesCount { get; set; }

        public SearchStatistics()
        {
            EvaluationStatistics = new EvaluationStatistics();
            PrincipalVariation = new Move[SearchConstants.MaxDepth];
        }
    }
}
