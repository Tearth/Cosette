# Version 2.0 (Darkness), --.--.----
 - Improved transposition tables: entries are now smaller, have proper checkmate scores (relative to position) and are used between moves (aging)
 - Added fifty-move rule detection
 - Fixed PV node detection
 - Added new evaluations functions: pawn shield, bishop pair, doubled rooks, a rook on open file
 - Fixed invalid detection of passing pawns
 - Added "evaluate" command to get FEN position evaluation
 - Added ability to postpone moves generation before PV move check
 - Improved move ordering: castling and better promotions are now prioritized
 - Added evaluation hash table
 - Redefined and reduced the size of Move structure (from 4 bytes to 2 bytes)
 - Reduced size of transposition table entry (from 16 bytes to 12 bytes), evaluation hash table entry (from 8 bytes to 4 bytes) and pawn hash table entry (from 8 bytes to 4 bytes)
 - Improved method of probing piece type at the specified field
 - Added extended futility pruning
 - Added Arbiter app to speed up the process of testing engine
 - Fixed invalid best move when search has been aborted
 - Improved time management - now allocated time depends from the moves count
 - Optimized printing UCI output
 - Fixed static exchange evaluation - in rare cases the table was returning invalid score

Estimated strength: ???? ELO

# Version 1.0 (Aqua), 19.09.2020
 - Initial version

Estimated strength: 1900 ELO