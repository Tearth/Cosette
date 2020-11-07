# Cosette
**Current version: v2.0 (Darkness), 19.10.2020**

A brand new UCI-compliant chess engine written in C# for .NET Core platform. The project is still in the early stage of development and lacks a few major performance improvements like more advanced pruning, extensions, or better evaluation. The current strength is estimated at 1950 ELO and hopefully will significantly improve in the future.

The engine has **[an official profile on CCRL](http://ccrl.chessdom.com/ccrl/404/cgi/compare_engines.cgi?family=Cosette&print=Rating+list&print=Results+table&print=LOS+table&print=Ponder+hit+table&print=Eval+difference+table&print=Comopp+gamenum+table&print=Overlap+table&print=Score+with+common+opponents)** (Computer Chess Rating Lists) where you can check the best strength estimation. Also, feel free to visit **[a dedicated forum thread](http://kirill-kryukov.com/chess/discussion-board/viewtopic.php?f=7&t=12402)** for Cosette releases and discussions!

![Cosette interactive console example](https://i.imgur.com/hIcaAmz.png)

## How to play?
The simplest way is to download the newest version from the **[Releases page](https://github.com/Tearth/Cosette/releases)** and use it with a graphical interface like Arena or WinBoard. The engine has been tested extensively on the first one, but should work with every UCI-compliant GUI.

Cosette has an official account on **[lichess.org](https://lichess.org/)** platform, which allows everybody to challenge her - feel free to do it!

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
 - doubled rooks
 - rooks on open files
 - bishop pair

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