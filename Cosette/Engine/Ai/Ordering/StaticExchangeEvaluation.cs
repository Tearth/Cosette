using System.Collections.Generic;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai.Ordering
{
    public static class StaticExchangeEvaluation
    {
        private static short[][][] _table;

        public static void Init()
        {
            InitTable();
            PopulateTable();
        }

        public static short Evaluate(int attackingPiece, int capturedPiece, int attacker, int defender)
        {
            return (short)(EvaluationConstants.Pieces[capturedPiece] + _table[attackingPiece][attacker][defender]);
        }

        private static void InitTable()
        {
            _table = new short[6][][];
            for (var attackingPiece = 0; attackingPiece < 6; attackingPiece++)
            {
                _table[attackingPiece] = new short[256][];
                for (var attackerIndex = 0; attackerIndex < 256; attackerIndex++)
                {
                    _table[attackingPiece][attackerIndex] = new short[256];
                }
            }
        }

        private static void PopulateTable()
        {
            var gainList = new List<int>();
            for (var attackingPiece = 0; attackingPiece < 6; attackingPiece++)
            {
                for (ulong attackerIndex = 0; attackerIndex < 256; attackerIndex++)
                {
                    for (ulong defenderIndex = 0; defenderIndex < 256; defenderIndex++)
                    {
                        var attackingPieceSeeIndex = GetSeeIndexByPiece(attackingPiece);
                        var attackers = attackerIndex & ~(1ul << attackingPieceSeeIndex);
                        var defenders = defenderIndex;

                        var currentPieceOnField = attackingPiece;
                        var result = 0;

                        gainList.Add(result);

                        if (defenders != 0)
                        {
                            var leastValuableDefenderPiece = GetLeastValuablePiece(defenders);
                            defenders = BitOperations.PopLsb(defenders);

                            result -= EvaluationConstants.Pieces[currentPieceOnField];
                            currentPieceOnField = leastValuableDefenderPiece;

                            gainList.Add(result);

                            while (attackers != 0)
                            {
                                var leastValuableAttackerPiece = GetLeastValuablePiece(attackers);
                                attackers = BitOperations.PopLsb(attackers);

                                result += EvaluationConstants.Pieces[currentPieceOnField];
                                currentPieceOnField = leastValuableAttackerPiece;

                                gainList.Add(result);

                                if (gainList[^1] > gainList[^3])
                                {
                                    result = gainList[^3];
                                    break;
                                }

                                if (defenders != 0)
                                {
                                    leastValuableDefenderPiece = GetLeastValuablePiece(defenders);
                                    defenders = BitOperations.PopLsb(defenders);

                                    result -= EvaluationConstants.Pieces[currentPieceOnField];
                                    currentPieceOnField = leastValuableDefenderPiece;

                                    gainList.Add(result);

                                    if (gainList[^1] < gainList[^3])
                                    {
                                        result = gainList[^3];
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        _table[attackingPiece][attackerIndex][defenderIndex] = (short)result;
                        gainList.Clear();
                    }
                }
            }
        }

        private static int GetPieceBySeeIndex(int index)
        {
            switch (index)
            {
                case SeePiece.Pawn: return Piece.Pawn;
                case SeePiece.Knight1: case SeePiece.Knight2: return Piece.Knight;
                case SeePiece.Bishop: return Piece.Bishop;
                case SeePiece.Rook1: case SeePiece.Rook2: return Piece.Rook;
                case SeePiece.Queen: return Piece.Queen;
                case SeePiece.King: return Piece.King;
            }

            return -1;
        }

        private static int GetSeeIndexByPiece(int piece)
        {
            switch (piece)
            {
                case Piece.Pawn: return SeePiece.Pawn;
                case Piece.Knight: return SeePiece.Knight1;
                case Piece.Bishop: return SeePiece.Bishop;
                case Piece.Rook: return SeePiece.Rook1;
                case Piece.Queen: return SeePiece.Queen;
                case Piece.King: return SeePiece.King;
            }

            return -1;
        }

        private static int GetLeastValuablePiece(ulong data)
        {
            var leastValuableDefenderField = BitOperations.GetLsb(data);
            var leastValuableDefenderPiece = BitOperations.BitScan(leastValuableDefenderField);

            return GetPieceBySeeIndex(leastValuableDefenderPiece);
        }
    }
}
