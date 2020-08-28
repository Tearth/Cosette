namespace Cosette.Engine.Ai
{
    public enum TranspositionTableEntryType : byte
    {
        Invalid,
        ExactScore,
        LowerBound,
        UpperBound
    }
}
