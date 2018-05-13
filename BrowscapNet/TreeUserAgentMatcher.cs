using System;
using System.Linq;

namespace net.loune.BrowscapNet
{
    public class TreeUserAgentMatcher : IIniHandler
    {
        public PatternDictionaryTree tree = new PatternDictionaryTree();

        public BrowserCapabilityInfo lastInfo;

        public TreeUserAgentMatcher()
        {
        }

        public void StartSection(string section, long lineNumber)
        {
            AddLastSection();

            lastInfo = new BrowserCapabilityInfo();
            lastInfo.Pattern = section;
            lastInfo.Rank = lineNumber;
        }

        public void KeyValue(string key, string value, long lineNumber)
        {

        }

        public string GetMatch(string userAgent)
        {
            var results = tree.FindAll(userAgent);
            return string.Join("\n", results.Select(m => m.pattern + " - " + m.item));
            //return tree.FindPatternIdentity(userAgent).part;
            //return "NA";
        }

        public BrowserCapabilityInfo Find(string userAgent)
        {
            var results = tree.FindAll(userAgent.ToLower());
            return (BrowserCapabilityInfo)results.OrderBy(r => ((BrowserCapabilityInfo)r.item).Rank).FirstOrDefault().item;
        }

        public void EndFile(long lineNumber)
        {
            AddLastSection();
        }

        private void AddLastSection()
        {
            if (lastInfo == null)
            {
                return;
            }

            tree.Add(lastInfo.Pattern.ToLower(), item: lastInfo);

            lastInfo = null;
        }
    }
}
