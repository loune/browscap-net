using System;
namespace BrowscapNet
{
    public interface IUserAgentMatcher
    {
        /// <summary>
        /// Finds the BrowserCapabilityInfo that matches this user agent string.
        /// </summary>
        /// <returns>The matching BrowserCapabilityInfo.</returns>
        /// <param name="userAgent">User agent string.</param>
        BrowserCapabilityInfo FindMatch(string userAgent);
    }
}
