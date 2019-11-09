namespace Porter2StemmerStandard
{
    public class IsExactlyLookupContainer : BTreeContainer
    {
        public IsExactlyLookupContainer(params (string word, string value)[] words) : base(words)
        {
        }

        public bool TryGetValue(string word, out string value)
        {
            var node = _root;

            value = default;

            foreach(var c in word)
            {
                node = node.Get(c);

                if (node == null) return false;
            }

            value = node.Value;
            return value != null;
        }
    }
}
