using System;

namespace Porter2StemmerStandard
{
    public class EndsWithContainer
    {
        public EndsWithContainer(params (string suffix, string value)[] suffixes)
        {
            _root = new LetterNode();

            foreach (var (suffix, value) in suffixes)
            {
                Insert(suffix, value);
            }
        }

        public EndsWithContainer(params string[] suffixes)
        {
            _root = new LetterNode();

            foreach (var suffix in suffixes)
            {
                Insert(suffix, null);
            }
        }

        public bool EndsWithAny(string word)
        {
            var node = _root;

            for (var i = word.Length - 1; i >= 0; i--)
            {
                node = node.Children[word[i]];

                if (node == null) return false;

                if (node.Fix != null) return true;
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
                node = node.Get(word[i]);

                if (node == null) break;

                if (node.Fix != null)
                {
                    suffix = node.Fix;
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
                var child = node.Children[key[i]];

                if(child == null)
                {
                    child = new LetterNode();
                    node.Children[key[i]] = child;
                }
                node = child;
            }

            if (node.Value != null) throw new ArgumentException($"Key '{key}' already in the collection");

            node.Value = value;
            node.Fix = key;
        }

        private readonly LetterNode _root;
    }
}
