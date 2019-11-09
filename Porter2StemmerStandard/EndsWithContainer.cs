using System.Collections.Generic;

namespace Porter2StemmerStandard
{
    public class EndsWithContainer : BTreeContainer
    {
        public EndsWithContainer(params (string suffix, string value)[] suffixes) : base(suffixes)
        {
        }

        public EndsWithContainer(params string[] suffixes) : base(suffixes)
        {
        }

        public bool EndsWithAny(string word)
        {
            var node = _root;

            for (var i = word.Length - 1; i >= 0; i--)
            {
                node = node.Children[word[i]];

                if (node == null) return false;

                if (node.Key != null) return true;
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

                if (node.Key != null)
                {
                    suffix = node.Key;
                    value = node.Value;
                }
            }

            return suffix != null;
        }

        // only call this from insert, the statemachine is too slow for use during actual operation
        protected override IEnumerable<char> GetChars(string word)
        {
            for (var i = word.Length - 1; i >= 0; i--)
            {
                yield return word[i];
            }
        }
    }
}
