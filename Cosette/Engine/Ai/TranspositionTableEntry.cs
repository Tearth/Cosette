using Cosette.Engine.Moves;

namespace Cosette.Engine.Ai
{
    public struct TranspositionTableEntry
    {
        public int Depth { get; set; }
        public int Score { get; set; }
        public Move BestMove { get; set; }
        public TranspositionTableEntryType Type { get; set; }

        public TranspositionTableEntry(int depth, int score, Move bestMove, TranspositionTableEntryType type)
        {
            Depth = depth;
            Score = score;
            BestMove = bestMove;
            Type = type;
        }
    }
}
