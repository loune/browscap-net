using System;
using BrowscapNet;
using Xunit;

namespace BrowscapNet.Tests
{
    public class BrowserCapabilitiesServiceTest
    {
        BrowserCapabilitiesService browserCapabilitiesService;
        public BrowserCapabilitiesServiceTest()
        {
            browserCapabilitiesService = new BrowserCapabilitiesService();
            browserCapabilitiesService.LoadBrowscap("testdata/full_asp_browscap.ini.gz");
        }

        [Theory]
        [InlineData("Mozilla/5.0 (Macintosh; Intel Mac OS X 10.13; rv:59.0) Gecko/20100101 Firefox/59.0", "Mozilla/5.0 (*Mac OS X 10?13*) Gecko* Firefox/59.0*", "Firefox", "59.0", "macOS", "Browser")]
        [InlineData("Mozilla/5.0 (iPhone; CPU iPhone OS 11_1_2 like Mac OS X) AppleWebKit/604.3.5 (KHTML, like Gecko) Version/11.0 Mobile/15B202 Safari/604.1", "mozilla/5.0 (iphone*cpu iphone os 11?1* like mac os x*)*applewebkit*(*khtml*like*gecko*)*version/11.0*safari/*", "Safari", "11.0", "iOS", "Browser")]
        [InlineData("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)", "mozilla/5.0 (compatible; googlebot/2.1*", "Google Bot", "2.1", "unknown", "Bot/Crawler")]
        [InlineData("Mozilla/5.0 (Linux; Android 6.0.1; Nexus 5 Build/MRA58N) AppleWebKit/537.36(KHTML, like Gecko) Chrome/61.0.3116.0 Mobile Safari/537.36 Chrome-Lighthouse", "mozilla/5.0 (linux; android * build/*) applewebkit*(*khtml*like*gecko*) chrome/* mobile safari/* chrome-lighthouse", "Google Lighthouse", "0.0", "unknown", "Bot/Crawler")]
        [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36 Edge/16.16299", "mozilla/5.0 (*windows nt 10.0*win64? x64*) applewebkit* (*khtml*like*gecko*) chrome/* safari/* edge/16.*", "Edge", "16.0", "Win10", "Browser")]
        public void ShouldFindBrowser(string userAgent, string pattern, string browser, string version, string platform, string browserType)
        {
            var info = browserCapabilitiesService.Find(userAgent);

            Assert.Equal(pattern.ToLower(), info.Pattern.ToLower());
            Assert.Equal(browser, info.Browser);
            Assert.Equal(version, info.Version);
            Assert.Equal(platform, info.Platform);
            Assert.Equal(browserType, info.BrowserType);
        }
    }
}
