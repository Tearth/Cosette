# Cosette
**Current version: v4.0 (Komekko), 22.01.2021**

An UCI-compliant chess engine written in C# for .NET Core platform, with **[an official profile on CCRL](http://ccrl.chessdom.com/ccrl/404/cgi/compare_engines.cgi?family=Cosette&print=Rating+list&print=Results+table&print=LOS+table&print=Ponder+hit+table&print=Eval+difference+table&print=Comopp+gamenum+table&print=Overlap+table&print=Score+with+common+opponents)** (Computer Chess Rating Lists) where you can check the best strength estimation. Feel free to visit **[a dedicated forum thread](http://kirill-kryukov.com/chess/discussion-board/viewtopic.php?f=7&t=12402)** for Cosette releases and discussions!

![Cosette interactive console example](https://i.imgur.com/nwPkim6.png)

On the table below you can see how engines' strength was changing with the versions. Take note that usually the latest one has an estimated value, which will be updated later to a more concrete result.

| Version                                                                | Release date | ELO   | Main changes |
|------------------------------------------------------------------------|--------------|-------| ------------ |
| [v4.0 (Komekko)](https://github.com/Tearth/Cosette/releases/tag/v4.0)  | 22.01.2021   | 2300* | Futility pruning, static null-move pruning, evasion move generator, check extensions, adjusted parameters of search and evaluation. |
| [v3.0 (Luna)](https://github.com/Tearth/Cosette/releases/tag/v3.0)     | 12.12.2020   | 2086  | Multi-stage move generation and move ordering, internal iterative deepening, SEE pruning in quiescence search. New evaluation function for fianchetto. |
| [v2.0 (Darkness)](https://github.com/Tearth/Cosette/releases/tag/v2.0) | 19.10.2020   | 1985  | A bunch of improvements for transposition tables, time management and move ordering. New evaluation functions: pawn shield, bishop pair, doubled rooks, a rook on the open file. |
| [v1.0 (Aqua)](https://github.com/Tearth/Cosette/releases/tag/v1.0)     | 19.09.2020   | 1875  | Initial release. |

**value estimated*

## How to play?
The simplest way is to download the newest version from the **[Releases page](https://github.com/Tearth/Cosette/releases)** and use it with a graphical interface like Arena or WinBoard. The engine has been tested extensively on the first one, but should work with every UCI-compliant GUI.

Cosette has an official account on **[lichess.org](https://lichess.org/)** platform, which allows everybody to challenge her - feel free to do it!

**https://lichess.org/@/CosetteBot**

## Algorithms

**Board representation and move generating:**
 - bitboards (with make/undo scheme)
 - magic bitboards
 - precalculated arrays with moves for knight and king
 - moves generator (quiet, loud, captures and evasions)

**Evaluation:**
 - material (incremental updating)
 - position (piece-square tables, incremental updating)
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
 - quiescence search with check detection
 - quiescence SEE pruning
 - iterative deepening
 - internal iterative deepening
 - staged move generating
 - null-move pruning
 - static null-move pruning
 - principal variation search
 - late move reduction
 - check extension
 - futility pruning

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

A set of console applications (selfplay + Texel) and web interface, which allows adjusting engine parameters using standard UCI command setoption. Results are displayed on the local website using a set of tables and charts.

![Cosette Tuner](https://i.imgur.com/uxXeYW9.png)

### Polyglot

The small library which allows using opening books saved in the Polyglot format. Feel free to use it if you need this in your own project.

## Why Cosette?

https://www.youtube.com/watch?v=XuYF-EnpLpc