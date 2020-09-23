using System;

namespace Cosette.Engine.Ai.Transposition
{
    public enum TranspositionTableEntryFlags : byte
    {
        Invalid = 0,
        ExactScore = 1,
        BetaScore = 2,
        AlphaScore = 4
    }
}
