namespace Porter2StemmerStandard
{
    public class BTreeNode
    {
        private const int TableSize = 'z' + 1;

        public BTreeNode[] Children { get; } = new BTreeNode[TableSize];
        public string Key { get; set; }
        public string Value { get; set; }

        public BTreeNode Get(char c)
        {
            if (c >= TableSize) return null;
            return Children[c];
        }
    }
}
