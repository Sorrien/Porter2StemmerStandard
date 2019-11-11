namespace Porter2StemmerStandard
{
    public class IsExactlyContainer : BTreeContainer
    {
        public IsExactlyContainer(params string[] words) : base(words)
        {
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
    }
}
