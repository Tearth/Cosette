using System;
using System.Collections.Generic;
using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Score;
using Cosette.Engine.Ai.Score.PieceSquareTables;
using Cosette.Engine.Ai.Search;
using Cosette.Engine.Ai.Transposition;
using Cosette.Engine.Common;

namespace Cosette.Uci.Commands
{
    public class SetOptionCommand : IUciCommand
    {
        private readonly UciClient _uciClient;
        private readonly Dictionary<string, Action<string>> _optionExecutors;

        public SetOptionCommand(UciClient uciClient)
        {
            _uciClient = uciClient;
            _optionExecutors = new Dictionary<string, Action<string>>
            {
                { "Hash", p => HashTableAllocator.Allocate(int.Parse(p)) },

                { "PawnValue", p => EvaluationConstants.Pieces[Piece.Pawn] = int.Parse(p) },
                { "KnightValue", p => EvaluationConstants.Pieces[Piece.Knight] = int.Parse(p) },
                { "BishopValue", p => EvaluationConstants.Pieces[Piece.Bishop] = int.Parse(p) },
                { "RookValue", p => EvaluationConstants.Pieces[Piece.Rook] = int.Parse(p) },
                { "QueenValue", p => EvaluationConstants.Pieces[Piece.Queen] = int.Parse(p) },
                { "KingValue", p => EvaluationConstants.Pieces[Piece.King] = int.Parse(p) },

                { "DoubledPawnsOpening", p => EvaluationConstants.DoubledPawns[GamePhase.Opening] = int.Parse(p) },
                { "DoubledPawnsEnding", p => EvaluationConstants.DoubledPawns[GamePhase.Ending] = int.Parse(p) },
                { "IsolatedPawnsOpening", p => EvaluationConstants.IsolatedPawns[GamePhase.Opening] = int.Parse(p) },
                { "IsolatedPawnsEnding", p => EvaluationConstants.IsolatedPawns[GamePhase.Ending] = int.Parse(p) },
                { "ChainedPawnsOpening", p => EvaluationConstants.ChainedPawns[GamePhase.Opening] = int.Parse(p) },
                { "ChainedPawnsEnding", p => EvaluationConstants.ChainedPawns[GamePhase.Ending] = int.Parse(p) },
                { "PassingPawnsOpening", p => EvaluationConstants.PassingPawns[GamePhase.Opening] = int.Parse(p) },
                { "PassingPawnsEnding", p => EvaluationConstants.PassingPawns[GamePhase.Ending] = int.Parse(p) },

                { "CenterMobilityModifier", p => EvaluationConstants.CenterMobilityModifier = int.Parse(p) },
                { "OutsideMobilityModifier", p => EvaluationConstants.OutsideMobilityModifier = int.Parse(p) },
                { "KingInDanger", p => EvaluationConstants.KingInDanger = int.Parse(p) },
                { "PawnShield", p => EvaluationConstants.PawnShield = int.Parse(p) },
                { "DoubledRooks", p => EvaluationConstants.DoubledRooks = int.Parse(p) },
                { "RookOnOpenFile", p => EvaluationConstants.RookOnOpenFile = int.Parse(p) },
                { "PairOfBishops", p => EvaluationConstants.PairOfBishops = int.Parse(p) },
                { "Fianchetto", p => EvaluationConstants.Fianchetto = int.Parse(p) },
                { "FianchettoWithoutBishop", p => EvaluationConstants.FianchettoWithoutBishop = int.Parse(p) },

                { "HashMove", p => MoveOrderingConstants.HashMove = short.Parse(p) },
                { "Promotion", p => MoveOrderingConstants.Promotion = short.Parse(p) },
                { "Castling", p => MoveOrderingConstants.Castling = short.Parse(p) },
                { "PawnNearPromotion", p => MoveOrderingConstants.PawnNearPromotion = short.Parse(p) },
                { "Capture", p => MoveOrderingConstants.Capture = short.Parse(p) },
                { "EnPassant", p => MoveOrderingConstants.EnPassant = short.Parse(p) },
                { "KillerMove", p => MoveOrderingConstants.KillerMove = short.Parse(p) },
                { "HistoryHeuristicMaxScore", p => MoveOrderingConstants.HistoryHeuristicMaxScore = uint.Parse(p) },
                { "KillerSlots", p => MoveOrderingConstants.KillerSlots = int.Parse(p) },

                { "IIDMinimalDepth", p => SearchConstants.IIDMinimalDepth = int.Parse(p) },
                { "IIDDepthReduction", p => SearchConstants.IIDDepthReduction = int.Parse(p) },

                { "StaticNullMoveMaximalDepth", p => SearchConstants.StaticNullMoveMaximalDepth = int.Parse(p) },
                { "StaticNullMoveMaximalDepthDivider", p => SearchConstants.StaticNullMoveMaximalDepthDivider = int.Parse(p) },
                { "StaticNullMoveMargin", p => SearchConstants.StaticNullMoveMargin = int.Parse(p) },
                { "StaticNullMoveMarginMultiplier", p => SearchConstants.StaticNullMoveMarginMultiplier = int.Parse(p) },

                { "NullMoveMinimalDepth", p => SearchConstants.NullMoveMinimalDepth = int.Parse(p) },
                { "NullMoveDepthReduction", p => SearchConstants.NullMoveDepthReduction = int.Parse(p) },
                { "NullMoveDepthReductionDivider", p => SearchConstants.NullMoveDepthReductionDivider = int.Parse(p) },

                { "FutilityPruningMaximalDepth", p => SearchConstants.FutilityPruningMaximalDepth = int.Parse(p) },
                { "FutilityPruningMaximalDepthDivisor", p => SearchConstants.FutilityPruningMaximalDepthDivisor = int.Parse(p) },
                { "FutilityPruningMargin", p => SearchConstants.FutilityPruningMargin = int.Parse(p) },
                { "FutilityPruningMarginMultiplier", p => SearchConstants.FutilityPruningMarginMultiplier = int.Parse(p) },

                { "LMRMinimalDepth", p => SearchConstants.LMRMinimalDepth = int.Parse(p) },
                { "LMRMovesWithoutReduction", p => SearchConstants.LMRMovesWithoutReduction = int.Parse(p) },
                { "LMRBaseReduction", p => SearchConstants.LMRBaseReduction = int.Parse(p) },
                { "LMRMoveIndexDivider", p => SearchConstants.LMRMoveIndexDivider = int.Parse(p) },
                { "LMRPvNodeMaxReduction", p => SearchConstants.LMRPvNodeMaxReduction = int.Parse(p) },
                { "LMRNonPvNodeMaxReduction", p => SearchConstants.LMRNonPvNodeMaxReduction = int.Parse(p) },

                { "Pawn.O0", p => PawnTables.O0 = int.Parse(p) },
                { "Pawn.O1", p => PawnTables.O1 = int.Parse(p) },
                { "Pawn.O2", p => PawnTables.O2 = int.Parse(p) },
                { "Pawn.O3", p => PawnTables.O3 = int.Parse(p) },
                { "Pawn.O4", p => PawnTables.O4 = int.Parse(p) },
                { "Pawn.O5", p => PawnTables.O5 = int.Parse(p) },
                { "Pawn.O6", p => PawnTables.O6 = int.Parse(p) },
                { "Pawn.E0", p => PawnTables.E0 = int.Parse(p) },
                { "Pawn.E1", p => PawnTables.E1 = int.Parse(p) },
                { "Pawn.E2", p => PawnTables.E2 = int.Parse(p) },
                { "Pawn.E3", p => PawnTables.E3 = int.Parse(p) },
                { "Pawn.E4", p => PawnTables.E4 = int.Parse(p) },

                { "Bishop.O0", p => BishopTables.O0 = int.Parse(p) },
                { "Bishop.O1", p => BishopTables.O1 = int.Parse(p) },
                { "Bishop.O2", p => BishopTables.O2 = int.Parse(p) },
                { "Bishop.O3", p => BishopTables.O3 = int.Parse(p) },
                { "Bishop.O4", p => BishopTables.O4 = int.Parse(p) },
                { "Bishop.O5", p => BishopTables.O5 = int.Parse(p) },
                { "Bishop.E0", p => BishopTables.E0 = int.Parse(p) },
                { "Bishop.E1", p => BishopTables.E1 = int.Parse(p) },
                { "Bishop.E2", p => BishopTables.E2 = int.Parse(p) },
                { "Bishop.E3", p => BishopTables.E3 = int.Parse(p) },
                { "Bishop.E4", p => BishopTables.E4 = int.Parse(p) },

                { "Knight.O0", p => KnightTables.O0 = int.Parse(p) },
                { "Knight.O1", p => KnightTables.O1 = int.Parse(p) },
                { "Knight.O2", p => KnightTables.O2 = int.Parse(p) },
                { "Knight.O3", p => KnightTables.O3 = int.Parse(p) },
                { "Knight.O4", p => KnightTables.O4 = int.Parse(p) },
                { "Knight.O5", p => KnightTables.O5 = int.Parse(p) },
                { "Knight.E0", p => KnightTables.E0 = int.Parse(p) },
                { "Knight.E1", p => KnightTables.E1 = int.Parse(p) },
                { "Knight.E2", p => KnightTables.E2 = int.Parse(p) },
                { "Knight.E3", p => KnightTables.E3 = int.Parse(p) },
                { "Knight.E4", p => KnightTables.E4 = int.Parse(p) },

                { "Rook.O0", p => RookTables.O0 = int.Parse(p) },
                { "Rook.O1", p => RookTables.O1 = int.Parse(p) },
                { "Rook.O2", p => RookTables.O2 = int.Parse(p) },
                { "Rook.O3", p => RookTables.O3 = int.Parse(p) },
                { "Rook.O4", p => RookTables.O4 = int.Parse(p) },
                { "Rook.E0", p => RookTables.E0 = int.Parse(p) },
                { "Rook.E1", p => RookTables.E1 = int.Parse(p) },
                { "Rook.E2", p => RookTables.E2 = int.Parse(p) },
                { "Rook.E3", p => RookTables.E3 = int.Parse(p) },

                { "Queen.O0", p => QueenTables.O0 = int.Parse(p) },
                { "Queen.O1", p => QueenTables.O1 = int.Parse(p) },
                { "Queen.O2", p => QueenTables.O2 = int.Parse(p) },
                { "Queen.O3", p => QueenTables.O3 = int.Parse(p) },
                { "Queen.O4", p => QueenTables.O4 = int.Parse(p) },
                { "Queen.O5", p => QueenTables.O5 = int.Parse(p) },
                { "Queen.E0", p => QueenTables.E0 = int.Parse(p) },
                { "Queen.E1", p => QueenTables.E1 = int.Parse(p) },
                { "Queen.E2", p => QueenTables.E2 = int.Parse(p) },
                { "Queen.E3", p => QueenTables.E3 = int.Parse(p) },
                { "Queen.E4", p => QueenTables.E4 = int.Parse(p) },

                { "King.O0", p => KingTables.O0 = int.Parse(p) },
                { "King.O1", p => KingTables.O1 = int.Parse(p) },
                { "King.O2", p => KingTables.O2 = int.Parse(p) },
                { "King.O3", p => KingTables.O3 = int.Parse(p) },
                { "King.O4", p => KingTables.O4 = int.Parse(p) },
                { "King.O5", p => KingTables.O5 = int.Parse(p) },
                { "King.E0", p => KingTables.E0 = int.Parse(p) },
                { "King.E1", p => KingTables.E1 = int.Parse(p) },
                { "King.E2", p => KingTables.E2 = int.Parse(p) },
                { "King.E3", p => KingTables.E3 = int.Parse(p) },
            };
        }

        public void Run(params string[] parameters)
        {
            var key = parameters[1];
            var value = parameters[3];

            if (_optionExecutors.ContainsKey(key))
            {
                _optionExecutors[key](value);

                // Value of material has changed, SEE table needs to be recalculated
                if (key.EndsWith("Value"))
                {
                    StaticExchangeEvaluation.Init();
                }
                
                PieceSquareTablesData.BuildPieceSquareTables();
            }
            else
            {
                _uciClient.SendError("badoption");
            }
        }
    }
}