using System;
using System.Collections.Generic;
using Cosette.Arbiter.Book;
using Xunit;

namespace Cosette.Arbiter.Tests
{
    public class HashKeyTests
    {
        [Theory]
        [InlineData(new string[] { }, 5060803636482931868)]
        public void HashKey_FromInitialPosition(string[] moves, ulong expectedHashKey)
        {
            var polyglotBoard = new PolyglotBoard();
            polyglotBoard.InitDefaultState();

            Assert.Equal(expectedHashKey, polyglotBoard.CalculateHash());
        }
    }
}
