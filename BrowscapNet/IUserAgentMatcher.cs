using System;
namespace BrowscapNet
{
    public interface IUserAgentMatcher
    {
        BrowserCapabilityInfo FindMatch(string userAgent);
    }
}
