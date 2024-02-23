using System;

namespace Porter2StemmerStandard
{
    public class StartsWithContainer : BTreeContainer
    {
        public StartsWithContainer(params string[] prefixes) : base(prefixes)
        {
        }

        public bool TryFindLongestPrefix(ReadOnlySpan<char> word, out string prefix)
        {
            var node = _root;

            prefix = default;

            foreach (var c in word)
            {
                node = node.Get(c);

                if (node == null) break;

                if (node.Key != null) prefix = node.Key;
            }

            return prefix != default;
        }
    }
}
