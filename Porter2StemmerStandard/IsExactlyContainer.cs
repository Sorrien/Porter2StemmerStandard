using System;

namespace Porter2StemmerStandard
{
    public class IsExactlyContainer
    {
        public IsExactlyContainer(params string[] words)
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

            return node.Key != null;
        }

        private void Insert(string word)
        {
            var node = _root;

            foreach (var c in word)
            {
                var child = node.Get(c);

                if (child == null)
                {
                    child = new LetterNode();
                    node.Children[c] = child;
                }
                node = child;
            }

            if (node.Key != null) throw new ArgumentException($"Word '{word}' already in the collection");

            node.Key = word;
        }

        private readonly LetterNode _root;
    }
}
