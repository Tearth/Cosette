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

                { "NullWindowMinimalDepth", p => SearchConstants.NullWindowMinimalDepth = int.Parse(p) },
                { "NullWindowDepthReduction", p => SearchConstants.NullWindowDepthReduction = int.Parse(p) },
                { "LMRMinimalDepth", p => SearchConstants.LMRMinimalDepth = int.Parse(p) },
                { "LMRMovesWithoutReduction", p => SearchConstants.LMRMovesWithoutReduction = int.Parse(p) },
                { "LMRPvNodeDepthReduction", p => SearchConstants.LMRPvNodeDepthReduction = int.Parse(p) },
                { "LMRNonPvNodeDepthDivisor", p => SearchConstants.LMRNonPvNodeDepthDivisor = int.Parse(p) },
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