using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Porter2StemmerStandard
{
    public class EndsWithContainer
    {
        public EndsWithContainer(IReadOnlyDictionary<string, string> suffixMap)
        {
            _root = new LetterNode();

            foreach (var kvp in suffixMap)
            {
                Insert(kvp.Key, kvp.Value);
            }
        }

        public EndsWithContainer(IEnumerable<string> suffixex)
        {
            _root = new LetterNode();

            foreach (var suffix in suffixex)
            {
                Insert(suffix, null);
            }
        }

        public bool EndsWithAny(string word)
        {
            var node = _root;

            for (var i = word.Length - 1; i >= 0; i--)
            {
                if (!node.Nodes.TryGetValue(word[i], out node))
                {
                    break;
                }

                if (node.Suffix != null)
                {
                    return true;
                }
            }

            return false;
        }

        public bool TryFindLongestSuffix(string word, out string suffix)
        {
            return TryFindLongestSuffixAndValue(word, out suffix, out var _);
        }

        public bool TryFindLongestSuffixAndValue(string word, out string suffix, out string value)
        {
            var node = _root;

            suffix = default;
            value = default;

            for (var i = word.Length - 1; i >= 0; i--)
            {
                if (!node.Nodes.TryGetValue(word[i], out node))
                {
                    break;
                }

                if (node.Suffix != null)
                {
                    suffix = node.Suffix;
                    value = node.Value;
                }
            }

            return suffix != null;
        }

        private void Insert(string key, string value)
        {
            var node = _root;

            for (var i = key.Length - 1; i >= 0; i--)
            {
                if (!node.Nodes.TryGetValue(key[i], out var nextNode))
                {
                    nextNode = new LetterNode
                    {
                        Letter = key[i]
                    };
                    node.Nodes.Add(key[i], nextNode);
                }
                node = nextNode;
            }

            if (node.Value != null) throw new ArgumentException($"Key '{key}' already in the collection");

            node.Value = value;
            node.Suffix = key;
        }

        private readonly LetterNode _root;

        [DebuggerDisplay("Node {Letter} {Value}")]
        private class LetterNode
        {
            public char Letter;
            public Dictionary<char, LetterNode> Nodes = new Dictionary<char, LetterNode>();
            public string Value;
            public string Suffix;
        }
    }
}
