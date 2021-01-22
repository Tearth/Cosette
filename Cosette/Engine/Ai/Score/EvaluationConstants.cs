namespace Cosette.Engine.Ai.Score
{
    public static class EvaluationConstants
    {
        public static int[] Pieces = { 100, 350, 370, 570, 1190, 20000 };

        public const int Checkmate = 32000;
        public const int ThreefoldRepetition = 0;
        public const int InsufficientMaterial = 0;

        public static int[] DoubledPawns = { -10, -30 };
        public static int[] IsolatedPawns = { -25, -5 };
        public static int[] ChainedPawns = { 4, 4 };
        public static int[] PassingPawns = { 0, 30 };

        public static int CenterMobilityModifier = 7;
        public static int OutsideMobilityModifier = 6;

        public static int KingInDanger = -10;
        public static int PawnShield = 20;

        public static int DoubledRooks = 40;
        public static int RookOnOpenFile = 50;
        public static int PairOfBishops = 50;

        public static int Fianchetto = 25;
        public static int FianchettoWithoutBishop = -25;

        public const int OpeningEndgameEdge = 20500;

        public const ulong Center = 0x1818000000;
        public const ulong ExtendedCenter = 0x3c3c3c3c0000;
        public const ulong ExtendedCenterRing = ExtendedCenter & ~Center;
        public const ulong Outside = 0xffffc3c3c3c3ffff;
    }
}
