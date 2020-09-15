using Cosette.Engine.Ai.Score;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Ordering
{
    public static class StaticExchangeEvaluation
    {
        private static int[][][][] _table;

        static StaticExchangeEvaluation()
        {
            InitTable();
            PopulateTable();
        }

        public static int Evaluate(Piece attackingPiece, Piece capturedPiece, byte attacker, byte defender)
        {
            return _table[(int)attackingPiece][(int)capturedPiece][attacker][defender];
        }

        private static void InitTable()
        {
            _table = new int[6][][][];
            for (var attackingPieceIndex = 0; attackingPieceIndex < 6; attackingPieceIndex++)
            {
                _table[attackingPieceIndex] = new int[6][][];
                for (var defendingPieceIndex = 0; defendingPieceIndex < 6; defendingPieceIndex++)
                {
                    _table[attackingPieceIndex][defendingPieceIndex] = new int[64][];
                    for (var defenderIndex = 0; defenderIndex < 64; defenderIndex++)
                    {
                        _table[attackingPieceIndex][defendingPieceIndex][defenderIndex] = new int[64];
                    }
                }
            }
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
                            var attackers = attackerIndex & ~(1ul << attackingPieceIndex);
                            var defenders = defenderIndex;

                            var currentPieceOnField = attackingPiece;
                            var result = EvaluationConstants.Pieces[(int)capturedPiece];

                            if (defenders != 0)
                            {
                                var leastValuableDefenderField = BitOperations.GetLsb(defenders);
                                defenders = (byte)BitOperations.PopLsb(defenders);
                                var leastValuableDefenderPiece = (Piece)BitOperations.BitScan(leastValuableDefenderField);

                                result -= EvaluationConstants.Pieces[(int)currentPieceOnField];
                                currentPieceOnField = leastValuableDefenderPiece;

                                while (attackers != 0)
                                {
                                    var updatedResult = result;
                                    var leastValuableAttackerField = BitOperations.GetLsb(attackers);
                                    attackers = (byte)BitOperations.PopLsb(attackers);
                                    var leastValuableAttackerPiece = (Piece)BitOperations.BitScan(leastValuableAttackerField);

                                    updatedResult += EvaluationConstants.Pieces[(int)currentPieceOnField];
                                    currentPieceOnField = leastValuableAttackerPiece;

                                    if (defenders != 0)
                                    {
                                        leastValuableDefenderField = BitOperations.GetLsb(defenders);
                                        defenders = (byte)BitOperations.PopLsb(defenders);
                                        leastValuableDefenderPiece = (Piece)BitOperations.BitScan(leastValuableDefenderField);

                                        updatedResult -= EvaluationConstants.Pieces[(int)currentPieceOnField];
                                        currentPieceOnField = leastValuableDefenderPiece;
                                    }
                                    else
                                    {
                                        result = updatedResult;
                                        break;
                                    }

                                    if (updatedResult < result)
                                    {
                                        break;
                                    }
                                }
                            }

                            _table[attackingPieceIndex][capturedPieceIndex][attackerIndex][defenderIndex] = result;
                        }
                    }
                }
            }
        }
    }
}
