using System;
using System.Collections.Generic;

namespace Porter2StemmerStandard
{
    public class IsExactlyContainer
    {

        public IsExactlyContainer(IEnumerable<string> words)
        {
            _root = new LetterNode();

            foreach (var word in words)
            {
                Insert(word);
            }
        }

        public bool Contains(string word)
        {
            var node = _root;

            for (var i = 0; i < word.Length; i++)
            {
                node = node.Get(word[i]);

                if (node == null) return false;
            }

            return node.Fix != null;
        }

        private void Insert(string word)
        {
            var node = _root;

            foreach(var c in word)
            {
                var child = node.Get(c);

                if(child == null)
                {
                    child = new LetterNode();
                    node.Children[c] = child;
                }
                node = child;
            }

            if (node.Fix != null) throw new ArgumentException($"Word '{word}' already in the collection");

            node.Fix = word;
        }

        private readonly LetterNode _root;
    }
}
