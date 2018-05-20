using System;
using System.IO;
using System.IO.Compression;

namespace BrowscapNet
{
    public class BrowserCapabilitiesService : IBrowserCapabilitiesService
    {
        private TreeUserAgentMatcher matcher;

        public BrowserCapabilitiesService()
        {
            matcher = new TreeUserAgentMatcher();
        }

        public void LoadBrowscap(string fileName)
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
                        LoadBrowscap(gzs);
                    }
                }
                else
                {
                    LoadBrowscap(fs);
                }
            }
        }

        public void LoadBrowscap(Stream stream)
        {
            new IniParser(matcher).Parse(stream);
        }

        public BrowserCapabilityInfo Find(string userAgent)
        {
            return matcher.FindMatch(userAgent);
        }
    }
}
