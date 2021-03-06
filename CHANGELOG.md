# Version 5.1 (Megumin), 07.06.2021
 - Fixed Shredder and Fritz issues

# Version 5.0 (Megumin), 12.04.2021
 - Added more detailed evaluation info in pawn hash table entry
 - Added futility pruning to quiescence search
 - Added reuse of killer table between moves
 - Added razoring
 - Added ability to stop the search if reliable mate score was found
 - Added search cut when enemy king is checked
 - Added detection of the open file next to the king in the evaluation
 - Added history heuristic veto in LMR
 - Added late move pruning
 - Fixed insufficient material detection when both sides had knight or bishop
 - Fixed threefold repetition detection
 - Fixed invalid depth in killer heuristic
 - Fixed memory usage issues during the search
 - Adjusted search parameters
 - Adjusted evaluation parameters
 - Allowed to use history heuristic values from the previous iterations
 - Allowed late move reduction when capture has a negative score
 - Improved time management when incrementation is present
 - Improved Fianchetto detection
 - Improved history table structure
 - Enabled support for BMI instruction set (only if CPU supports it)
 - Disabled futility pruning when enemy king is checked

# Version 4.0 (Komekko), 22.01.2021
 - Added Texel project
 - Added check extension
 - Added check detection in quiescence search
 - Added evasion moves generator
 - Added static null-move pruning
 - Added futility pruning
 - Added new formulas calculating reduction for LMR and null move pruning
 - Added a new way of building Piece-Square tables, better for tuning
 - Fixed reduntant saving of transposition table entries
 - Adjusted search parameters
 - Adjusted evaluation parameters
 - Improved threefold repetition detection
 - Updated .NET runtime version to 5.0.102

# Version 3.0 (Luna), 12.12.2020
 - Added better time control for Arbiter
 - Added more UCI options
 - Added Tuner project
 - Added insufficient material detection
 - Added executable hash generator
 - Added abort when search lasts too long
 - Added legality checking of the hash table moves
 - Added SEE pruning in the quiescence search
 - Added fianchetto evaluation
 - Added internal iterative deepening
 - Added a lot of UCI options, allowing for full engine customization
 - Added multi-stage move ordering
 - Added multi-stage move generating
 - Fixed FEN parser when input didn't have halfmove clock and moves count
 - Fixed crash when the engine was receiving invalid position in UCI mode
 - Fixed UCI statistics
 - Improved time scheduler when incrementation time is present
 - Improved mobility calculation by rewarding for center control
 - Improved late move reduction conditions
 - Improved SEE accuracy (now includes x-ray attacks)
 - Improved king safety evaluation
 - Changed maximal moves count from 128 to 218 (according to Internet sources)
 - Reduced size of transposition table entry (from 12 bytes to 8 bytes)
 - Disabled most of the evaluation functions when the game is near to end
 - Disabled returning of exact transposition table entries in the PV nodes
 - Adjusted evaluation scores
 - Optimized king safety evaluation
 - Updated .NET runtime version to 5.0.100

# Version 2.0 (Darkness), 19.10.2020
 - Added fifty-move rule detection
 - Added new evaluation functions: pawn shield, bishop pair, doubled rooks, a rook on open file
 - Added "evaluate" command to get FEN position evaluation
 - Added ability to postpone moves generation before PV move check
 - Added evaluation hash table
 - Added Arbiter app to speed up the process of testing engine
 - Added support for UCI's winc and binc
 - Fixed PV node detection
 - Fixed invalid detection of passing pawns
 - Fixed invalid best move when a search has been aborted
 - Fixed static exchange evaluation - in rare cases the table was returning an invalid score
 - Improved method of probing piece type at the specified field
 - Improved time management - now allocated time depends on the moves count
 - Improved move ordering: castling and better promotions are now prioritized
 - Improved transposition tables: entries are now smaller, have proper checkmate scores (relative to position) and are used between moves (aging)
 - Redefined and reduced the size of Move structure (from 4 bytes to 2 bytes)
 - Reduced size of transposition table entry (from 16 bytes to 12 bytes), evaluation hash table entry (from 8 bytes to 4 bytes) and pawn hash table entry (from 8 bytes to 4 bytes)
 - Optimized printing UCI output
 - Adjusted move ordering scores
 - Updated .NET Core runtime version to 3.1.403

# Version 1.0 (Aqua), 19.09.2020
 - Initial version