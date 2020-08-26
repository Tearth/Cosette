﻿using System.Runtime.CompilerServices;
using Cosette.Engine.Board;
using Cosette.Engine.Common;

namespace Cosette.Engine.Ai
{
    public class Evaluation
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Evaluate(BoardState board, Color color)
        {
            var sign = color == Color.White ? 1 : -1;
            return sign * board.Material;
        }
    }
}