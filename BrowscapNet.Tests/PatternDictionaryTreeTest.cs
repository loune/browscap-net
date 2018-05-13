using System;
using System.Linq;
using Xunit;

namespace net.loune.BrowscapNet.Tests
{
    public class PatternDictionaryTreeTest
    {
        [Theory]
        [InlineData("*", "abc")]
        [InlineData("?", "a")]
        [InlineData("a*c", "abc")]
        [InlineData("a?c", "abc")]
        [InlineData("a??c", "aabc")]
        [InlineData("???", "abc")]
        [InlineData("abc*", "abc")]
        [InlineData("*abc", "abc")]
        [InlineData("*a*b*c*", "abc")]
        [InlineData("Mozilla/5.0 (*Mac OS X 10?13*) Gecko* Firefox/59.0", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.13; rv:59.0) Gecko/20100101 Firefox/59.0")]
        [InlineData("Mozilla/5.0 (*Mac OS X 10?13*) Gecko* Firefox/59.0", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.13; rv:59.0) Gecko/20100101 Firefox/59.0")]
        public void PatternShouldMatch(string pattern, string input)
        {
            PatternDictionaryTree tree = new PatternDictionaryTree();
            tree.Add(pattern);
            Assert.Equal(pattern, tree.Find(input).First().part);
        }

        [Theory]
        [InlineData("a?c", "abbc")]
        [InlineData("a?c", "ac")]
        [InlineData("*a*b*c*", "ab")]
        [InlineData("abc", "yyabc")]
        [InlineData("abc", "abcd")]
        [InlineData("???", "abcd")]
        [InlineData("zzzzzz*yyyyyyy", "abcd")]
        [InlineData("Mozilla/5.0 (*Mac OS X 10?13*) Gecko* Firefox/59.0", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.14; rv:59.0) Gecko/20100101 Firefox/59.0")]
        public void PatternShouldNotMatch(string pattern, string input)
        {
            PatternDictionaryTree tree = new PatternDictionaryTree();
            tree.Add(pattern);
            Assert.Equal(null, tree.Find(input));
        }
    }
}
