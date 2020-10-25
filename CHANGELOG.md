# Version 3.0 (----), --.--.----
 - Changed maximal moves count from 128 to 218 (according to Internet sources)
 - Fixed FEN parser when input didn't have halfmove clock and moves count
 - Added abort when search lasts too long
 - Improved time scheduler when incrementation time is present
 - Improved mobility calculation by rewarding for center control
 - Added better time control for Arbiter

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