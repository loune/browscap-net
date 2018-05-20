using System;
using System.Linq;
using Xunit;

namespace BrowscapNet.Tests
{
    public class PatternDictionaryTreeTest
    {
        PatternDictionaryTree tree;

        public PatternDictionaryTreeTest()
        {
            tree = new PatternDictionaryTree();
        }

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
            tree.Add(pattern);
            Assert.Equal(0, tree.FindAll(input).Count);
        }

        [Theory]
        [InlineData("a?c", "a?c")]
        [InlineData("Mozilla/5.0 (* Mac OS X 10?12*) Gecko* Firefox/59.0*", "Mozilla/5.0 (* Mac OS X 10?12*) Gecko* Firefox/59.0*")]
        public void PatternShouldFindIdentity(string pattern, string input)
        {
            var item = new object();
            tree.Add(pattern, item);
            var result = tree.FindPatternIdentity(input);
            Assert.Equal(pattern, result.pattern);
            Assert.Equal(item, result.item);
        }

        [Fact]
        public void ShouldFindPatternInTreeWithMultipleItems()
        {
            string google = "mozilla/5.0 (iphone*cpu iphone os 9?1* like mac os x*) applewebkit* (*khtml*like*gecko*) version/* mobile/* safari/* (compatible; adsbot-google-mobile; +http://www.google.com/mobile/adsbot.html)";
            object googleItem = new object();
            tree.Add("Mozilla/5.0 (*Linux*Android?4.3*Galaxy Nexus Build/*)*applewebkit* (*khtml*like*gecko*) Chrome/25.0*Safari/*");
            tree.Add("Mozilla/5.0 (* Mac OS X 10?12*) Gecko* Firefox/59.0*", "Mozilla/5.0 (* Mac OS X 10?12*) Gecko* Firefox/59.0*");
            tree.Add(google, googleItem);
            var results = tree.FindAll("Mozilla/5.0 (iPhone; CPU iPhone OS 9_1 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Version/9.0 Mobile/13B143 Safari/601.1 (compatible; AdsBot-Google-Mobile; +http://www.google.com/mobile/adsbot.html)".ToLower());
            var result = results.Single();
            Assert.Equal(google, result.pattern);
            Assert.Equal(googleItem, result.item);

        }
    }
}
