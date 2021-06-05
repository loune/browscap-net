using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace BrowscapNet
{
    /// <summary>
    /// Browser capabilities service. Load the browscap.ini file and finds BrowserCapabilityInfo based on user agent strings.
    /// </summary>
    public class BrowserCapabilitiesService : IBrowserCapabilitiesService
    {
        private TreeUserAgentMatcher matcher;
        private SemaphoreSlim matcherLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Gets the browscap.ini version.
        /// </summary>
        /// <value>The browscap version.</value>
        public string BrowscapVersion => matcher.BrowscapVersion;

        /// <summary>
        /// Gets the browscap.ini released date.
        /// </summary>
        /// <value>The browscap released.</value>
        public DateTimeOffset BrowscapReleased => matcher.BrowscapReleased;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:BrowscapNet.BrowserCapabilitiesService"/> class.
        /// </summary>
        public BrowserCapabilitiesService()
        {
            matcher = new TreeUserAgentMatcher();
        }

        /// <summary>
        /// Loads browscap.ini from filename. Can be gzipped.
        /// </summary>
        /// <param name="fileName">File path of full_asp_browscap.ini.</param>
        public async Task LoadBrowscap(string fileName)
        {
            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                byte[] header = new byte[2];
                fs.Read(header, 0, 2);
                fs.Seek(0, SeekOrigin.Begin);

                bool isGzip = (header[0] == 0x1f && header[1] == 0x8b);

                if (isGzip)
                {
                    using (var gzs = new GZipStream(fs, CompressionMode.Decompress))
                    {
                        await LoadBrowscap(gzs);
                    }
                }
                else
                {
                    await LoadBrowscap(fs);
                }
            }
        }

        /// <summary>
        /// Load browscap.ini from a stream.
        /// </summary>
        /// <param name="stream">Stream.</param>
        public async Task LoadBrowscap(Stream stream)
        {
            await matcherLock.WaitAsync();
            try
            {
                await new IniParser(matcher).ParseAsync(stream);
            }
            finally
            {
                matcherLock.Release();
            }
        }

        /// <summary>
        /// Find the specified userAgent.
        /// </summary>
        /// <returns>The Browser Capability Info.</returns>
        /// <param name="userAgent">User agent.</param>
        public BrowserCapabilityInfo Find(string userAgent)
        {
            return matcher.FindMatch(userAgent);
        }
    }
}
