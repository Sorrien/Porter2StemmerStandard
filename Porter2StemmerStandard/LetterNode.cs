namespace Porter2StemmerStandard
{
    public class LetterNode
    {
        private const int TableSize = 'z' + 1;

        public LetterNode[] Children { get; } = new LetterNode[TableSize];
        public string Fix { get; set; }
        public string Value { get; set; }

        public LetterNode Get(char c)
        {
            if (c >= TableSize) return null;

            unchecked
            {
                return Children[c];
            }
        }
    }
}
