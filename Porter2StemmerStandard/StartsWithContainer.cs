using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Porter2StemmerStandard
{
    public class StartsWithContainer
    {
        public StartsWithContainer(IEnumerable<string> prefixes)
        {
            _root = new LetterNode();

            foreach (var prefix in prefixes)
            {
                Insert(prefix);
            }
        }

        public bool TryFindLongestPrefix(string word, out string prefix)
        {
            var node = _root;

            prefix = default;

            for (var i = 0; i < word.Length; i++)
            {
                if (!node.Nodes.TryGetValue(word[i], out node))
                {
                    break;
                }

                if (node.Prefix != null)
                {
                    prefix = node.Prefix;
                }
            }

            return prefix != default;
        }

        private void Insert(string key)
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

            if (node.Prefix != null) throw new ArgumentException($"Key '{key}' already in the collection");

            node.Prefix = key;
        }

        private readonly LetterNode _root;

        [DebuggerDisplay("Node {Letter} {Value}")]
        private class LetterNode
        {
            public char Letter;
            public Dictionary<char, LetterNode> Nodes = new Dictionary<char, LetterNode>();
            public string Prefix;
        }
    }
}
