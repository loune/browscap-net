using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace net.loune.BrowscapNet
{
    public class PatternDictionaryTree
    {
        public bool ExactMatch { get; set; } = false;
        public long Rank { get; set; }
        //public Dictionary<string, PatternDictionaryTree>[] patternPrefixes;
        public List<(char wildcardChar, int patternLen, Dictionary<string, PatternDictionaryTree> patterns)> patternPrefixes;
        private int maxLength;

        public PatternDictionaryTree(int maxLength = 200)
        {
            this.maxLength = maxLength;
        }

        private string GetPrefix(string userAgent)
        {
            for (int i = 0; i < userAgent.Length; i++)
            {
                if (userAgent[i] == '*' || userAgent[i] == '?')
                {
                    return userAgent.Substring(0, i);
                }
            }

            return userAgent;
        }

        public void Add(string pattern, char wildcardChar = '^', long rank = 0)
        {
            var prefix = GetPrefix(pattern);
            if (prefix.Length > maxLength)
            {
                throw new ArgumentException("pattern length is too long");
            }

            if (patternPrefixes == null)
            {
                //patternPrefixes = new Dictionary<string, PatternDictionaryTree>[maxLength];
                patternPrefixes = new List<(char, int, Dictionary<string, PatternDictionaryTree>)>();
            }

            //if (patternPrefixes[prefix.Length] == null)
            //{
            //  patternPrefixes[prefix.Length] = new Dictionary<string, PatternDictionaryTree>();
            //}

            var lenDictionary = patternPrefixes.FirstOrDefault(p => p.wildcardChar == wildcardChar && p.patternLen == prefix.Length);
            if (lenDictionary.wildcardChar == default(char))
            {
                lenDictionary = (wildcardChar, prefix.Length, new Dictionary<string, PatternDictionaryTree>());
                patternPrefixes.Add(lenDictionary);
            }

            if (!lenDictionary.patterns.TryGetValue(prefix, out PatternDictionaryTree tree))
            {
                tree = new PatternDictionaryTree(maxLength)
                {
                    Rank = rank
                };

                lenDictionary.patterns[prefix] = tree;
            }

            if (pattern.Length != prefix.Length /*&& !(pattern[prefix.Length] == '?' && pattern.Length == 1)*/)
            {
                tree.Add(pattern.Substring(prefix.Length + 1), pattern[prefix.Length], rank);
            }
            else
            {
                tree.ExactMatch = true;
                tree.Rank = rank;
            }
        }

        public List<(char wildcardChar, long rank, string part)> Find(string input)
        {
            if (patternPrefixes == null)
            {
                return null;
            }

            var inputLen = input.Length;
            List<(char, long, string)> results = null;
            //for (int i = input.Length; i >= 0; i--)
            foreach (var lenDictionary in patternPrefixes)
            {
                var (wildcardChar, i, patterns) = lenDictionary;
                if (i > inputLen) {
                    continue;
                }
                //if (patternPrefixes[i] != null)
                {
                    bool beginningCharWildcard = wildcardChar == '^';
                    bool singleCharWildcard = wildcardChar == '?';
                    var max = singleCharWildcard && i < inputLen ? (1 + i) : inputLen;
                    if (beginningCharWildcard)
                    {
                        max = i;
                    }

                    for (int j = singleCharWildcard ? 1 : 0; j + i <= max; j++)
                    {
                        var prefix = i == 0 ? string.Empty : input.Substring(j, i);
                        Debug.Assert(prefix.Length == i);

                        if (patterns.TryGetValue(prefix, out PatternDictionaryTree tree))
                        {
                            var remaining = input.Substring(j + i);
                            if ((remaining.Length == 0 && tree.ExactMatch) /* we've reached "*abc"  */ ||
                                (prefix.Length == 0 && tree.patternPrefixes == null && !singleCharWildcard) /* we've reached "*abc*" */ )
                            {
                                if (results == null)
                                    results = new List<(char, long, string)>();
                                results.Add((wildcardChar, tree.Rank, prefix));
                                break;
                                //return results;
                            }
                            else
                            {
                                var matches = tree.Find(remaining);

                                if (matches != null)
                                {
                                    if (results == null)
                                        results = new List<(char, long, string)>();
                                    results.AddRange(matches.Select(m => (wildcardChar, m.rank, prefix + m.wildcardChar + m.part)));
                                    //return results;
                                }
                            }
                        }
                    }
                }
            }

            return results;
        }
    }
}
