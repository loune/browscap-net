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
        public List<(char wildcardChar, int patternLen, Dictionary<string, PatternDictionaryTree> patterns)> patternPrefixes;

        public PatternDictionaryTree()
        {
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

            if (patternPrefixes == null)
            {
                patternPrefixes = new List<(char, int, Dictionary<string, PatternDictionaryTree>)>();
            }

            var lenDictionary = patternPrefixes.FirstOrDefault(p => p.wildcardChar == wildcardChar && p.patternLen == prefix.Length);
            if (lenDictionary.wildcardChar == default(char))
            {
                lenDictionary = (wildcardChar, prefix.Length, new Dictionary<string, PatternDictionaryTree>());
                patternPrefixes.Add(lenDictionary);
            }

            if (!lenDictionary.patterns.TryGetValue(prefix, out PatternDictionaryTree tree))
            {
                tree = new PatternDictionaryTree()
                {
                    Rank = rank
                };

                lenDictionary.patterns[prefix] = tree;
            }

            if (pattern.Length != prefix.Length)
            {
                tree.Add(pattern.Substring(prefix.Length + 1), pattern[prefix.Length], rank);
            }
            else
            {
                tree.ExactMatch = true;
                tree.Rank = rank;
            }
        }

        public (char wildcardChar, long rank, string part) FindPatternIdentity(string pattern)
        {
            var list = Find(pattern, FindMode.SearchIdentityStart);
            return list != null ? list.FirstOrDefault() : default((char wildcardChar, long rank, string part));
        }

        public List<(char wildcardChar, long rank, string part)> FindAll(string input)
        {
            return Find(input);
        }

        private List<(char wildcardChar, long rank, string part)> Find(string input, FindMode mode = FindMode.None)
        {
            if (patternPrefixes == null)
            {
                return null;
            }

            bool findIdenity = mode == FindMode.SearchIdentity || mode == FindMode.SearchIdentityStart;
            string identityPrefix = null;
            if (findIdenity)
            {
                identityPrefix = GetPrefix(mode == FindMode.SearchIdentityStart ? input : input.Substring(1));
            }

            var inputLen = input.Length;
            List<(char, long, string)> results = null;
            foreach (var lenDictionary in patternPrefixes)
            {
                var (wildcardChar, patternLen, patterns) = lenDictionary;
                if (patternLen > inputLen)
                {
                    // our pattern is bigger than the input, won't match
                    continue;
                }

                if (findIdenity && (patternLen != identityPrefix.Length || (mode != FindMode.SearchIdentityStart && wildcardChar != input[0])))
                {
                    // if finding identity, we are only interested in an exact part in the exact length
                    continue;
                }

                bool beginningCharWildcard = wildcardChar == '^';
                bool singleCharWildcard = wildcardChar == '?' || (findIdenity && !beginningCharWildcard);
                int startIndex = 0;
                var max = inputLen;
                if (singleCharWildcard)
                {
                    // ? matches one char exactly - only search by first character of input
                    startIndex = 1;
                    max = 1 + patternLen;
                    if (max > inputLen)
                    {
                        // our pattern is bigger than the input, won't match
                        continue;
                    }
                }
                else if (beginningCharWildcard)
                {
                    // we are at the beginning of the input, we should only match from the start of the input
                    max = patternLen;
                }

                for (; startIndex + patternLen <= max; startIndex++)
                {
                    var prefix = patternLen == 0 ? string.Empty : input.Substring(startIndex, patternLen);
                    Debug.Assert(prefix.Length == patternLen);

                    if (patterns.TryGetValue(prefix, out PatternDictionaryTree tree))
                    {
                        var remaining = input.Substring(startIndex + patternLen);
                        if ((remaining.Length == 0 && tree.ExactMatch) /* we've reached "*abc"  */ ||
                            (prefix.Length == 0 && tree.patternPrefixes == null && !singleCharWildcard) /* we've reached "*abc*" */ )
                        {
                            if (results == null)
                            {
                                results = new List<(char, long, string)>();
                            }

                            results.Add((wildcardChar, tree.Rank, prefix));

                            if (findIdenity)
                            {
                                return results;
                            }

                            break;
                        }
                        else
                        {
                            var matches = tree.Find(remaining, findIdenity ? FindMode.SearchIdentity : mode);

                            if (matches != null)
                            {
                                if (results == null)
                                {
                                    results = new List<(char, long, string)>();
                                }

                                results.AddRange(matches.Select(m => (wildcardChar, m.rank, prefix + m.wildcardChar + m.part)));

                                if (findIdenity)
                                {
                                    return results;
                                }
                            }
                        }
                    }

                }
            }

            return results;
        }

        private enum FindMode
        {
            None,
            SearchIdentityStart,
            SearchIdentity,
            SingleResult
        }
    }
}
