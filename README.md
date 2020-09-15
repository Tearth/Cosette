# Cosette

A brand new UCI-compliant chess engine written in C# for .NET Core platform. The project is still in the early stage of development and lacks a few major performance improvements like more advanced pruning, extensions, or better evaluation. The current strength is estimated at 1900 ELO and hopefully will significantly improve in the future.

## How to play?
Cosette has an official account on lichess.org platform, which allows everybody to challenge her - feel free to do it!

**https://lichess.org/@/CosetteBot**

## Algorithms

**Board representation and move generating:**
 - bitboards (with make/undo scheme)
 - magic bitboards
 - precalculated arrays with moves for knight and king

**Evaluation:**
 - material (incremental updating)
 - position (piece-square tables, incremental updating)
 - castling status
 - mobility
 - king safety
 - pawn structure (cached in pawn hash table)

**Search:**
 - negamax
 - alpha-beta pruning
 - transposition table
 - quiescence search
 - iterative deepening
 - null-move pruning
 - principal variation search
 - late move reduction

**Move ordering:**
 - static exchange evaluation
 - killer heuristic
 - history heuristic

## Why Cosette?

https://www.youtube.com/watch?v=XuYF-EnpLpc