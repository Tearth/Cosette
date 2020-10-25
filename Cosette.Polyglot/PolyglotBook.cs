using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Cosette.Polyglot.Book;

namespace Cosette.Polyglot
{
    public class PolyglotBook
    {
        private string _bookFile;
        private Random _random;

        public PolyglotBook(string bookFile)
        {
            _bookFile = bookFile;
            _random = new Random();
        }

        public List<string> GetRandomOpening(int movesCount)
        {
            var movesList = new List<PolyglotBookEntry>();
            var polyglotBoard = new PolyglotBoard();
            polyglotBoard.InitDefaultState();

            for (var moveIndex = 0; moveIndex < movesCount; moveIndex++)
            {
                var availableMoves = GetBookEntries(polyglotBoard.CalculateHash());
                if (availableMoves.Count == 0)
                {
                    break;
                }

                availableMoves = availableMoves.OrderBy(p => p.Weight).ToList();
                var weightSum = availableMoves.Sum(p => p.Weight);

                var probabilityArray = new double[availableMoves.Count];
                for (var availableMoveIndex = 0; availableMoveIndex < availableMoves.Count; availableMoveIndex++)
                {
                    probabilityArray[availableMoveIndex] = (double)availableMoves[availableMoveIndex].Weight / weightSum;
                }

                var randomValue = _random.NextDouble();
                for (var availableMoveIndex = 0; availableMoveIndex < availableMoves.Count; availableMoveIndex++)
                {
                    if (probabilityArray[availableMoveIndex] > randomValue || availableMoveIndex == availableMoves.Count - 1)
                    {
                        movesList.Add(availableMoves[availableMoveIndex]);
                        polyglotBoard.MakeMove(availableMoves[availableMoveIndex].Move.ToString());
                        break;
                    }
                }
            }

            return movesList.Select(p => p.Move.ToString()).ToList();
        }

        public unsafe List<PolyglotBookEntry> GetBookEntries(ulong hash)
        {
            var foundEntries = new List<PolyglotBookEntry>();

            var entrySize = sizeof(PolyglotBookEntry);
            var bookInfo = new FileInfo(_bookFile);
            var entriesCount = bookInfo.Length / entrySize;
            long left = 0;
            long right = entriesCount - 1;

            using (var binaryReader = new BinaryReader(new FileStream(_bookFile, FileMode.Open)))
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
