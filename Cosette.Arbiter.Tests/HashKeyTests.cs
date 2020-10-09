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
        [InlineData(new[] { "e2e4" }, 9384546495678726550)]
        [InlineData(new[] { "e2e4", "d7d5" }, 528813709611831216)]
        [InlineData(new[] { "e2e4", "d7d5", "e4e5", }, 7363297126586722772)]
        [InlineData(new[] { "e2e4", "d7d5", "e4e5", "f7f5" }, 2496273314520498040)]
        [InlineData(new[] { "e2e4", "d7d5", "e4e5", "f7f5", "e1e2" }, 7289745035295343297)]
        [InlineData(new[] { "e2e4", "d7d5", "e4e5", "f7f5", "e1e2", "e8f7" }, 71445182323015129)]
        [InlineData(new[] { "a2a4", "b7b5", "h2h4", "b5b4", "c2c4" }, 4359805404264691255)]
        [InlineData(new[] { "a2a4", "b7b5", "h2h4", "b5b4", "c2c4", "b4c3", "a1a3" }, 6647202560273257824)]
        public void HashKey_FromInitialPosition(string[] moves, ulong expectedHashKey)
        {
            var polyglotBoard = new PolyglotBoard();
            polyglotBoard.InitDefaultState();

            Assert.Equal(expectedHashKey, polyglotBoard.CalculateHash());
        }
    }
}
