using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Cosette.Arbiter.Settings;

namespace Cosette.Arbiter.Book
{
    public class PolyglotBook
    {
        private Random _random;

        public PolyglotBook()
        {
            _random = new Random();
        }

        public List<PolyglotBookEntry> GetRandomOpening()
        {
            var movesList = new List<PolyglotBookEntry>();
            var polyglotBoard = new PolyglotBoard();
            polyglotBoard.InitDefaultState();

            for (var i = 0; i < SettingsLoader.Data.PolyglotMaxMoves; i++)
            {
                var availableMoves = GetBookEntries(polyglotBoard.CalculateHash());
                if (availableMoves.Count == 0)
                {
                    break;
                }

                var entry = availableMoves[_random.Next(0, availableMoves.Count)];
                movesList.Add(entry);

                polyglotBoard.MakeMove(entry.Move.ToString());
            }

            return movesList;
        }

        public unsafe List<PolyglotBookEntry> GetBookEntries(ulong hash)
        {
            var foundEntries = new List<PolyglotBookEntry>();

            var entrySize = sizeof(PolyglotBookEntry);
            var bookInfo = new FileInfo(SettingsLoader.Data.PolyglotOpeningBook);
            var entriesCount = bookInfo.Length / entrySize;
            long left = 0;
            long right = entriesCount - 1;

            using (var binaryReader = new BinaryReader(new FileStream(SettingsLoader.Data.PolyglotOpeningBook, FileMode.Open)))
            {
                var buffer = new byte[16];
                var bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                var bufferPtr = bufferHandle.AddrOfPinnedObject();

                while (left <= right)
                {
                    long middle = (left + right) / 2;
                    var entry = ReadEntry(binaryReader, buffer, bufferPtr, middle, entrySize);

                    if (entry.Hash < hash)
                    {
                        left = middle + 1;
                    }
                    else
                    {
                        right = middle - 1;
                    }
                }

                while (true)
                {
                    var entry = ReadEntry(binaryReader, buffer, bufferPtr, left++, entrySize);
                    if (entry.Hash == hash)
                    {
                        if (entry.Move != PolyglotBookMove.Zero)
                        {
                            foundEntries.Add(entry);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                bufferHandle.Free();
            }

            return foundEntries;
        }

        private PolyglotBookEntry ReadEntry(BinaryReader binaryReader, byte[] buffer, IntPtr bufferPtr, long position, int entrySize)
        {
            binaryReader.BaseStream.Seek(position * entrySize, SeekOrigin.Begin);
            binaryReader.Read(buffer, 0, 16);

            // Swap big endian to little endian
            Array.Reverse(buffer, 0, 8);
            Array.Reverse(buffer, 8, 2);
            Array.Reverse(buffer, 10, 2);
            Array.Reverse(buffer, 12, 4);

            return (PolyglotBookEntry)Marshal.PtrToStructure(bufferPtr, typeof(PolyglotBookEntry));
        }
    }
}
