namespace Porter2StemmerStandard
{
    public static class Letters
    {
        private static readonly bool[] _table;

        static Letters()
        {
            _table = new bool['z' + 1];

            foreach(var c in "aeiouy")
            {
                _table[c] = true;
            }
        }

        public static bool IsVowel(char c)
        {
            if (c >= _table.Length) return false;
            unchecked
            {
                return _table[c];
            }
        }

        public static bool IsConsonant(char c) => !IsVowel(c);
    }
}
