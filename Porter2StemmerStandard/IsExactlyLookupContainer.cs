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

            for (var i = 0; i < word.Length; i++)
            {
                node = node.Get(word[i]);

                if (node == null) return false;
            }

            value = node.Value;
            return value != null;
        }
    }
}
