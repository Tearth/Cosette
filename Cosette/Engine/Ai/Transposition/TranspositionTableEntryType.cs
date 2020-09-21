namespace Cosette.Engine.Ai.Transposition
{
    public enum TranspositionTableEntryType : byte
    {
        Invalid,
        ExactScore,
        BetaScore,
        AlphaScore
    }
}
