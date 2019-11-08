using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Porter2StemmerStandard
{
    public class StartsWithContainer
    {
        public StartsWithContainer(IReadOnlyDictionary<string, string> prefixMap)
        {
            _root = new LetterNode();

            foreach (var kvp in prefixMap)
            {
                Insert(kvp.Key, kvp.Value);
            }
        }

        public StartsWithContainer(IEnumerable<string> prefixes)
        {
            _root = new LetterNode();

            foreach (var prefix in prefixes)
            {
                Insert(prefix, null);
            }
        }

        public IReadOnlyList<(string Prefix, string Value)> Check(string word)
        {
            var node = _root;

            var matches = new List<(string Prefix, string Value)>();

            for (var i = 0; i < word.Length; i++)
            {
                if (!node.Nodes.TryGetValue(word[i], out node))
                {
                    break;
                }

                if (node.Prefix != null)
                {
                    matches.Add((node.Prefix, node.Value));
                }
            }

            matches.Reverse();

            return matches;
        }

        private void Insert(string key, string value)
        {
            var node = _root;

            for (var i = 0; i < key.Length; i++)
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
            node.Prefix = key;
        }

        private readonly LetterNode _root;

        [DebuggerDisplay("Node {Letter} {Value}")]
        private class LetterNode
        {
            public char Letter;
            public Dictionary<char, LetterNode> Nodes = new Dictionary<char, LetterNode>();
            public string Value;
            public string Prefix;
        }
    }
}
