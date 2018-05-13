using System;
using System.IO;

namespace net.loune.BrowscapNet
{
    public class IniParser
    {
        private IIniHandler handler;

        public IniParser(IIniHandler handler)
        {
            this.handler = handler;
        }

        public void Parse(Stream stream)
        {
            using (StreamReader sr = new StreamReader(stream))
            {
                long lineNumber = 0;
                while (!sr.EndOfStream)
                {
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
                        handler.StartSection(name, lineNumber);
                    }
                    else
                    {
                        // key/value
                        int eqi = line.IndexOf('=');
                        if (eqi != -1)
                        {
                            var key = line.Substring(0, eqi);
                            var value = line.Substring(eqi + 1);
                            handler.KeyValue(key, value);
                        }
                    }
                }
            }
        }
    }

    public interface IIniHandler
    {
        void StartSection(string section, long lineNumber);
        void KeyValue(string key, string value);
    }
}
