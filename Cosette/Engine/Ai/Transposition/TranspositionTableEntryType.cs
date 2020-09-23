using System;

namespace Cosette.Engine.Ai.Transposition
{
    [Flags]
    public enum TranspositionTableEntryType : byte
    {
        Invalid = 0,
        ExactScore = 1,
        BetaScore = 2,
        AlphaScore = 4,
        Old = 8
    }
}
