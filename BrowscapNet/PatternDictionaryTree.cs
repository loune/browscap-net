using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BrowscapNet
{
    public class PatternDictionaryTree
    {
        public bool ExactMatch { get; set; } = false;
        public object Item { get; set; }
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

        public void Add(string pattern, object item = null, char wildcardChar = '^')
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
                    Item = item
                };

                lenDictionary.patterns[prefix] = tree;
            }

            if (pattern.Length != prefix.Length)
            {
                tree.Add(pattern.Substring(prefix.Length + 1), item, pattern[prefix.Length]);
            }
            else
            {
                tree.ExactMatch = true;
                tree.Item = item;
            }
        }

        public (string pattern, object item) FindPatternIdentity(string pattern)
        {
            var list = Find(pattern, FindMode.SearchIdentityStart);
            return list != null ? (list.First().part, list.First().item) : default((string part, object item));
        }

        public List<(string pattern, object item)> FindAll(string input)
        {
            var list = Find(input);
            return list != null ? list.Select(((char wildcardChar, object item, string part) m) => (m.part, m.item)).ToList() : new List<(string part, object item)>();
        }

        private List<(char wildcardChar, object item, string part)> Find(string input, FindMode mode = FindMode.None)
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
            List<(char, object, string)> results = null;
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
                    // and make sure that first char matches the wildcardChar
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
                                results = new List<(char, object, string)>();
                            }

                            results.Add((wildcardChar, tree.Item, prefix));

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
                                    results = new List<(char, object, string)>();
                                }

                                results.AddRange(matches.Select(m => (wildcardChar, m.item, prefix + m.wildcardChar + m.part)));

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
