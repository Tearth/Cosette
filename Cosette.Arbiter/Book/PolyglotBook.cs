using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Cosette.Arbiter.Settings;

namespace Cosette.Arbiter.Book
{
    public class PolyglotBook
    {
        public List<string> GetRandomOpening()
        {
            return null;
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
                        foundEntries.Add(entry);
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
