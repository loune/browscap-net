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
        [InlineData("a?c", "a?c")]
        [InlineData("Mozilla/5.0 (*Mac OS X 10?13*) Gecko* Firefox/59.0", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10.13; rv:59.0) Gecko/20100101 Firefox/59.0")]
        public void PatternShouldMatch(string pattern, string input)
        {
            var item = new object();
            PatternDictionaryTree tree = new PatternDictionaryTree();
            tree.Add(pattern, item);
            var result = tree.FindAll(input).First();
            Assert.Equal(pattern, result.pattern);
            Assert.Equal(item, result.item);
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
            Assert.Equal(0, tree.FindAll(input).Count);
        }


        [Theory]
        [InlineData("a?c", "a?c")]
        [InlineData("Mozilla/5.0 (* Mac OS X 10?12*) Gecko* Firefox/59.0*", "Mozilla/5.0 (* Mac OS X 10?12*) Gecko* Firefox/59.0*")]
        public void PatternShouldFindIdentity(string pattern, string input)
        {
            var item = new object();
            PatternDictionaryTree tree = new PatternDictionaryTree();
            tree.Add(pattern, item);
            var result = tree.FindPatternIdentity(input);
            Assert.Equal(pattern, result.pattern);
            Assert.Equal(item, result.item);
        }
    }
}
