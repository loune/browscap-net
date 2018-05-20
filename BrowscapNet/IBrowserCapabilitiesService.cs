using System;
namespace BrowscapNet
{
    public interface IBrowserCapabilitiesService
    {
        BrowserCapabilityInfo Find(string userAgent);
    }
}
