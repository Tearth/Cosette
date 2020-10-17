# Version 2.0 (Darkness), --.--.----
 - Added fifty-move rule detection
 - Added new evaluations functions: pawn shield, bishop pair, doubled rooks, a rook on open file
 - Added "evaluate" command to get FEN position evaluation
 - Added ability to postpone moves generation before PV move check
 - Added evaluation hash table
 - Added extended futility pruning
 - Added Arbiter app to speed up the process of testing engine
 - Fixed PV node detection
 - Fixed invalid detection of passing pawns
 - Fixed invalid best move when search has been aborted
 - Fixed static exchange evaluation - in rare cases the table was returning invalid score
 - Improved method of probing piece type at the specified field
 - Improved time management - now allocated time depends from the moves count
 - Improved move ordering: castling and better promotions are now prioritized
 - Improved transposition tables: entries are now smaller, have proper checkmate scores (relative to position) and are used between moves (aging)
 - Redefined and reduced the size of Move structure (from 4 bytes to 2 bytes)
 - Reduced size of transposition table entry (from 16 bytes to 12 bytes), evaluation hash table entry (from 8 bytes to 4 bytes) and pawn hash table entry (from 8 bytes to 4 bytes)
 - Optimized printing UCI output
 - Adjusted move ordering scores

Estimated strength: ???? ELO

# Version 1.0 (Aqua), 19.09.2020
 - Initial version

Estimated strength: 1900 ELO