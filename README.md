# BrowscapNET - User Agent Browser Capabilities Detection for .NET

BrowscapNET is a .NET Standard 2.1 library that detects browser capabilities such as browser name, version and platform based on a supplied user agent string. It has a fast implementation of browser capbilities detection using `full_asp_browscap.ini` from [browscap.org](http://browscap.org). Searching a database of 150,000 user agent patterns from [full_asp_browscap.ini](http://browscap.org/stream?q=Full_BrowsCapINI) takes ~5ms on Macbook Pro 2017 per query uncached.

This library was primarily developed to quickly parse user agents from log files. Existing user agent parser solutions were either unreliable, didn't provide enough details, or used regular expressions that would take up to 1 second to return a result. This solution doesn't use regular expressions. Instead it constructs a pattern tree from all the patterns, allowing it to quickly seek a matching pattern with the help of a hash table.

As the initial use case was bulk log parsing, initialisation time and memory usage was sacificed for runtime performance. However, these could be further optimised in the future.

## Usage

A copy of `full_asp_browscap.ini` from [browscap.org](http://browscap.org) needs to be loaded into `BrowserCapabilitiesService` via the `LoadBrowscap` method. After loading the data, you can call `Find` to get `BrowserCapabilityInfo`. See `BrowserCapabilityInfo.cs` for the full list of capabilities.

It is recommended that you cache the results of `Find` for better performance.

### Example

```csharp
BrowserCapabilitiesService browserCapabilitiesService = new BrowserCapabilitiesService();
browserCapabilitiesService.LoadBrowscap("full_asp_browscap.ini");
var info = browserCapabilitiesService.Find("Mozilla/5.0 (Macintosh; Intel Mac OS X 10.13; rv:59.0) Gecko/20100101 Firefox/59.0");
Console.WriteLine(info.Browser); // Firefox
Console.WriteLine(info.Version); // 59
```

See the BrowserCapabilityInfo class for a full list of capabilties.

## Alternative Browser Detection

- [HttpBrowserCapabilities](https://docs.microsoft.com/en-us/dotnet/api/system.web.httpbrowsercapabilities)
- https://github.com/ua-parser
- https://github.com/woothee
- https://github.com/browscap/browscap-php
- https://github.com/crossjoin/Browscap
- https://github.com/matomo-org/device-detector
- https://github.com/WhichBrowser

### Comparisons

- https://github.com/kenjis/user-agent-parser-benchmarks
- https://github.com/diablomedia/useragent-parser-comparison
