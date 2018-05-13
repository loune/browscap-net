using System;
using System.Linq;

namespace net.loune.BrowscapNet
{
    public class TreeUserAgentMatcher : IIniHandler
    {
        public PatternDictionaryTree tree = new PatternDictionaryTree();

        public TreeUserAgentMatcher()
        {
        }

        public void StartSection(string section, long lineNumber)
        {
            tree.Add(section, rank: lineNumber);
        }

        public void KeyValue(string key, string value)
        {

        }

        public string GetMatch(string ua)
        {
            return string.Join("\n", tree.Find(ua).Select(m => m.part + " - " + m.rank));
            //return "NA";
        }
    }
}
