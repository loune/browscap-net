using System;
using System.IO;

namespace BrowscapNet
{
    /// <summary>
    /// A parser for an ini file. Provide an IIniHandler implementation.
    /// </summary>
    public class IniParser
    {
        private IIniHandler handler;

        public IniParser(IIniHandler handler)
        {
            this.handler = handler;
        }

        /// <summary>
        /// Parse the specified ini file stream.
        /// </summary>
        /// <param name="stream">The ini file stream.</param>
        public void Parse(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                long lineNumber = 0;
                while (!sr.EndOfStream)
                {
                    bool cancelParsing = false;
                    lineNumber++;
                    string line = sr.ReadLine();
                    if (line.Length == 0)
                    {
                        continue;
                    }

                    if (line[0] == ';')
                    {
                        // comment

                    }
                    else if (line[0] == '[')
                    {
                        // section
                        string name = line.Substring(1, line.IndexOf(']') - 1);
                        handler.StartSection(name, lineNumber, out cancelParsing);
                    }
                    else
                    {
                        // key/value
                        int eqi = line.IndexOf('=');
                        if (eqi != -1)
                        {
                            var key = line.Substring(0, eqi);
                            var value = line.Substring(eqi + 1);
                            handler.KeyValue(key, value, lineNumber, out cancelParsing);
                        }
                    }

                    if (cancelParsing)
                    {
                        return;
                    }
                }

                handler.EndFile(lineNumber);
            }
        }
    }

    /// <summary>
    /// Ini handler.
    /// </summary>
    public interface IIniHandler
    {
        /// <summary>
        /// Called when IniParser encounters a new section.
        /// </summary>
        /// <param name="section">The name of the section.</param>
        /// <param name="lineNumber">The line number of the section.</param>
        /// <param name="cancelParsing">If set to <c>true</c>, cancel the parsing operation.</param>
        void StartSection(string section, long lineNumber, out bool cancelParsing);

        /// <summary>
        /// Called when IniParser encounters each key value pair.
        /// </summary>
        /// <param name="key">Key name.</param>
        /// <param name="value">Value.</param>
        /// <param name="lineNumber">The line number of the key value pair.</param>
        /// <param name="cancelParsing">If set to <c>true</c>, cancel the parsing operation.</param>
        void KeyValue(string key, string value, long lineNumber, out bool cancelParsing);

        /// <summary>
        /// Called when IniParser encounters the end of the file.
        /// </summary>
        /// <param name="lineNumber">The line number of the last line.</param>
        void EndFile(long lineNumber);
    }
}
