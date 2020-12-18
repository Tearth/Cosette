namespace Cosette.Engine.Ai.Score
{
    public static class EvaluationConstants
    {
        public static int[] Pieces = { 100, 320, 330, 500, 1100, 20000 };

        public const int Checkmate = 32000;
        public const int ThreefoldRepetition = 0;
        public const int InsufficientMaterial = 0;

        public static int CastlingDone = 50;
        public static int CastlingFailed = -50;

        public static int[] DoubledPawns = { -20, -10 };
        public static int[] IsolatedPawns = { -20, -10 };
        public static int[] ChainedPawns = { 5, 5 };
        public static int[] PassingPawns = { 10, 50 };

        public static int CenterMobilityModifier = 10;
        public static int OutsideMobilityModifier = 5;

        public static int KingInDanger = -10;
        public static int PawnShield = 15;

        public static int DoubledRooks = 40;
        public static int RookOnOpenFile = 20;
        public static int PairOfBishops = 30;

        public static int Fianchetto = 20;
        public static int FianchettoWithoutBishop = -20;

        public const int OpeningEndgameEdge = 20500;

        public const ulong Center = 0x3c3c3c3c0000;
        public const ulong Outside = 0xffffc3c3c3c3ffff;
    }
}
