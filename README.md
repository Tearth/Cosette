# Cosette
**Current version: v3.0 (Luna), 12.12.2020**

An UCI-compliant chess engine written in C# for .NET Core platform, with **[an official profile on CCRL](http://ccrl.chessdom.com/ccrl/404/cgi/compare_engines.cgi?family=Cosette&print=Rating+list&print=Results+table&print=LOS+table&print=Ponder+hit+table&print=Eval+difference+table&print=Comopp+gamenum+table&print=Overlap+table&print=Score+with+common+opponents)** (Computer Chess Rating Lists) where you can check the best strength estimation. Feel free to visit **[a dedicated forum thread](http://kirill-kryukov.com/chess/discussion-board/viewtopic.php?f=7&t=12402)** for Cosette releases and discussions!

![Cosette interactive console example](https://i.imgur.com/2cSfVBR.png)

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
 - fianchetto
 - tapering

**Search:**
 - negamax
 - alpha-beta pruning
 - transposition table
 - quiescence search
 - quiescence SEE pruning
 - iterative deepening
 - internal iterative deepening
 - staged move generating
 - null-move pruning
 - principal variation search
 - late move reduction

**Move ordering:**
 - staged move ordering
 - static exchange evaluation
 - killer heuristic
 - history heuristic

## Additional tools

### Arbiter

The simple console application which allows testing chess engines using super-fast time control (like 2s+20"). It has been designed to support Cosette development, but should be also usable with other UCI-compliant engines.

![Cosette Arbiter](https://i.imgur.com/m7rYtuf.png)

### Tuner

The console application + web interface, which allows adjusting engine parameters using standard UCI command setoption. Results are displayed on the local website using a set of tables and charts.

![Cosette Tuner](https://i.imgur.com/uxXeYW9.png)

### Polyglot

The small library which allows using opening books saved in the Polyglot format. Feel free to use it if you need this in your own project.

## Why Cosette?

https://www.youtube.com/watch?v=XuYF-EnpLpc