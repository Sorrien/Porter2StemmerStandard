using System;
using System.Collections.Generic;

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

            foreach(var c in word)
            {
                node = node.Get(c);

                if (node == null) break;

                if (node.Fix != null) prefix = node.Fix;
            }

            return prefix != default;
        }

        private void Insert(string key)
        {
            var node = _root;

            foreach(var c in key)
            {
                var child = node.Children[c];

                if(child == null)
                {
                    child = new LetterNode();
                    node.Children[c] = child;
                }
                node = child;
            }

            if (node.Fix != null) throw new ArgumentException($"Key '{key}' already in the collection");

            node.Fix = key;
        }

        private readonly LetterNode _root;
    }
}
