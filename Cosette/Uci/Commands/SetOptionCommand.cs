using System;
using System.Collections.Generic;
using Cosette.Engine.Ai.Ordering;
using Cosette.Engine.Ai.Score;
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
                { "PawnAdvancesOpening", p => EvaluationConstants.PawnAdvances[GamePhase.Opening] = int.Parse(p) },
                { "PawnAdvancesEnding", p => EvaluationConstants.PawnAdvances[GamePhase.Ending] = int.Parse(p) },

                { "CastlingDone", p => EvaluationConstants.CastlingDone = int.Parse(p) },
                { "CastlingFailed", p => EvaluationConstants.CastlingFailed = int.Parse(p) },
                { "CenterMobilityModifier", p => EvaluationConstants.CenterMobilityModifier = int.Parse(p) },
                { "OutsideMobilityModifier", p => EvaluationConstants.OutsideMobilityModifier = int.Parse(p) },
                { "KingInDanger", p => EvaluationConstants.KingInDanger = int.Parse(p) },
                { "PawnShield", p => EvaluationConstants.PawnShield = int.Parse(p) },
                { "DoubledRooks", p => EvaluationConstants.DoubledRooks = int.Parse(p) },
                { "RookOnOpenFile", p => EvaluationConstants.RookOnOpenFile = int.Parse(p) },
                { "PairOfBishops", p => EvaluationConstants.PairOfBishops = int.Parse(p) },
                { "Fianchetto", p => EvaluationConstants.Fianchetto = int.Parse(p) },
                { "FianchettoWithoutBishop", p => EvaluationConstants.FianchettoWithoutBishop = int.Parse(p) },
                { "KingCentrismOpening", p => EvaluationConstants.KingCentrism[GamePhase.Opening] = int.Parse(p) },
                { "KingCentrismEnding", p => EvaluationConstants.KingCentrism[GamePhase.Ending] = int.Parse(p) },
                { "CenterControl", p => EvaluationConstants.CenterControl = int.Parse(p) },
                { "PieceOnEdge", p => EvaluationConstants.PieceOnEdge = int.Parse(p) },
                { "RookOnSeventhRank", p => EvaluationConstants.RookOnSeventhRank = int.Parse(p) },

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

                { "NullWindowMinimalDepth", p => SearchConstants.NullWindowMinimalDepth = int.Parse(p) },
                { "NullWindowBaseDepthReduction", p => SearchConstants.NullWindowBaseDepthReduction = int.Parse(p) },
                { "NullWindowDepthDivider", p => SearchConstants.NullWindowDepthDivider = int.Parse(p) },

                { "LMRMinimalDepth", p => SearchConstants.LMRMinimalDepth = int.Parse(p) },
                { "LMRMovesWithoutReduction", p => SearchConstants.LMRMovesWithoutReduction = int.Parse(p) },
                { "LMRBaseReduction", p => SearchConstants.LMRBaseReduction = int.Parse(p) },
                { "LMRMoveIndexDivider", p => SearchConstants.LMRMoveIndexDivider = int.Parse(p) },
                { "LMRPvNodeMaxReduction", p => SearchConstants.LMRPvNodeMaxReduction = int.Parse(p) },
                { "LMRNonPvNodeMaxReduction", p => SearchConstants.LMRNonPvNodeMaxReduction = int.Parse(p) },
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
            }
            else
            {
                _uciClient.SendError("badoption");
            }
        }
    }
}