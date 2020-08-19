namespace Cosette.Engine.Moves.Magic
{
    public struct MagicContainer
    {
        public ulong Mask { get; set; }
        public ulong MagicNumber { get; set; }
        public ulong[] Attacks { get; set; }
        public int Shift { get; set; }
    }
}
