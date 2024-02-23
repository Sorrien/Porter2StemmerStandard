using System;

namespace Porter2StemmerStandard
{
    public class IsExactlyContainer : BTreeContainer
    {
        public IsExactlyContainer(params string[] words) : base(words)
        {
        }

        public bool Contains(ReadOnlySpan<char> word)
        {
            var node = _root;

            foreach (var c in word)
            {
                node = node.Get(c);

                if (node == null) return false;
            }

            return node.Key != null;
        }
    }
}
