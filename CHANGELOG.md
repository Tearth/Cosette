# Version 4.0 (------), --.--.----
 - Added Texel project
 - Adjusted search parameters
 - Adjusted evaluation parameters
 - Fixed reduntant saving of transposition table entries
 - Added new formulas calculating reduction for LMR and NW pruning

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
 - Updated .NET Core runtime version to 5.0.100

Estimated strength: 2000 ELO

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

Estimated strength: 1950 ELO

# Version 1.0 (Aqua), 19.09.2020
 - Initial version

Estimated strength: 1900 ELO