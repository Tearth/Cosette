using Cosette.Engine.Ai.Score;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Ordering
{
    public static class StaticExchangeEvaluation
    {
        private static int[][][][] _table;

        public static void Init()
        {
            _table = InitTable();
            PopulateTable();
        }

        public static int Evaluate(Piece attackingPiece, Piece capturedPiece, byte attacker, byte defender)
        {
            return _table[(int)attackingPiece][(int)capturedPiece][attacker][defender];
        }

        private static int[][][][] InitTable()
        {
            var result = new int[6][][][];
            for (var attackingPieceIndex = 0; attackingPieceIndex < 6; attackingPieceIndex++)
            {
                result[attackingPieceIndex] = new int[6][][];
                for (var defendingPieceIndex = 0; defendingPieceIndex < 6; defendingPieceIndex++)
                {
                    result[attackingPieceIndex][defendingPieceIndex] = new int[64][];
                    for (var defenderIndex = 0; defenderIndex < 64; defenderIndex++)
                    {
                        result[attackingPieceIndex][defendingPieceIndex][defenderIndex] = new int[64];
                    }
                }
            }

            return result;
        }

        private static void PopulateTable()
        {
            for (var attackingPieceIndex = 0; attackingPieceIndex < 6; attackingPieceIndex++)
            {
                for (var capturedPieceIndex = 0; capturedPieceIndex < 6; capturedPieceIndex++)
                {
                    var attackingPiece = (Piece) attackingPieceIndex;
                    var capturedPiece = (Piece) capturedPieceIndex;

                    for (ulong attackerIndex = 0; attackerIndex < 64; attackerIndex++)
                    {
                        for (ulong defenderIndex = 0; defenderIndex < 64; defenderIndex++)
                        {
                            var attacker = attackerIndex;
                            var defender = defenderIndex;

                            var currentPieceOnField = attackingPiece;
                            var result = EvaluationConstants.Pieces[(int)capturedPiece];
                            var lastResult = result;

                            while (defender != 0)
                            {
                                var leastValuableDefenderField = BitOperations.GetLsb(defender);
                                defender = (byte)BitOperations.PopLsb(defender);
                                var leastValuableDefenderPiece = (Piece)BitOperations.BitScan(leastValuableDefenderField);

                                result -= EvaluationConstants.Pieces[(int)currentPieceOnField];
                                currentPieceOnField = leastValuableDefenderPiece;

                                if (attacker != 0)
                                {
                                    var leastValuableAttackerField = BitOperations.GetLsb(attacker);
                                    attacker = (byte)BitOperations.PopLsb(attacker);
                                    var leastValuableAttackerPiece = (Piece)BitOperations.BitScan(leastValuableAttackerField);

                                    result += EvaluationConstants.Pieces[(int)currentPieceOnField];
                                    currentPieceOnField = leastValuableAttackerPiece;
                                }
                                else
                                {
                                    break;
                                }

                                if (result < lastResult)
                                {
                                    break;
                                }

                                lastResult = result;
                            }

                            _table[attackingPieceIndex][capturedPieceIndex][attackerIndex][defenderIndex] = lastResult;
                        }
                    }
                }
            }
        }
    }
}
