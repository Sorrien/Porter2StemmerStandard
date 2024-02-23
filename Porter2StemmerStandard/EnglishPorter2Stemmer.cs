using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Porter2StemmerStandard
{
    /// <summary>
    /// Based off of the improved Porter2 algorithm:
    /// http://snowball.tartarus.org/algorithms/english/stemmer.html
    /// </summary>
    public class EnglishPorter2Stemmer : IPorter2Stemmer
    {

        private static readonly char[] _alphabet =
            Enumerable
                .Range('a', 'z' - 'a' + 1)
                .Select(c => (char)c)
                .Concat(new[] { '\'' }).ToArray();
        public ReadOnlySpan<char> Alphabet { get { return _alphabet; } }

        private static readonly char[] _vowels = "aeiouy".ToArray();
        public ReadOnlySpan<char> Vowels { get { return _vowels; } }

        private static readonly string[] _doubles =
            { "bb", "dd", "ff", "gg", "mm", "nn", "pp", "rr", "tt" };
        private static readonly EndsWithContainer _doublesEndsWith = new EndsWithContainer(_doubles);
        public ReadOnlySpan<string> Doubles { get { return _doubles; } }

        private static readonly HashSet<char> _liEndings = new HashSet<char>("cdeghkmnrt");
        public ReadOnlySpan<char> LiEndings { get { return _liEndings.ToArray(); } }

        private static readonly char[] _nonShortConsonants = "wxY".ToArray();

        private static readonly IsExactlyLookupContainer _exceptions = new IsExactlyLookupContainer(
            ("skis", "ski"),
            ("skies", "sky"),
            ("dying", "die"),
            ("lying", "lie"),
            ("tying", "tie"),
            ("idly", "idl"),
            ("gently", "gentl"),
            ("ugly", "ugli"),
            ("early", "earli"),
            ("only", "onli"),
            ("singly", "singl"),
            ("sky", "sky"),
            ("news", "news"),
            ("howe", "howe"),
            ("atlas", "atlas"),
            ("cosmos", "cosmos"),
            ("bias", "bias"),
            ("andes", "andes")
        );

        private static readonly IsExactlyContainer _exceptionsPart2 = new IsExactlyContainer(
            "inning", "outing", "canning", "herring", "earring",
            "proceed", "exceed", "succeed");

        private static readonly StartsWithContainer _exceptionsRegion1 = new StartsWithContainer(
            "gener", "arsen", "commun");


        /// <summary>
        /// Stems a word, returning both the original word and the stem.
        /// </summary>
        public StemmedWord Stem(string word)
        {
            Span<char> buffer = stackalloc char[word.Length];
            var length = ToLowerInvariant(word.AsSpan(), buffer);
            var wordSpan = buffer.Slice(0, length);

            StemInternal(ref wordSpan);
            return new StemmedWord(wordSpan.ToString(), word);
        }

        private static int ToLowerInvariant(ReadOnlySpan<char> source, Span<char> destination)
        {
            // At least on .NET Framework, ToLowerInvariant allocates strings internally.
            // return source.ToLowerInvariant(destination);
            for (var i = 0; i < source.Length; i++)
            {
                destination[i] = char.ToLowerInvariant(source[i]);
            }
            return source.Length;
        }

        private static void ToLowerInvariant(Span<char> buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                buffer[i] = char.ToLowerInvariant(buffer[i]);
            }
        }

        /// <summary>
        /// Stems a word, leaving the stem in the provided output buffer and
        /// returning the relevant slice of that buffer.
        /// </summary>
        /// <param name="word">Input word to stem.</param>
        /// <param name="output">Output buffer. Always modified. Must be large enough to hold <paramref name="word"/>.</param>
        /// <returns>The slice of the output buffer which contains the stemmed word.</returns>
        public ReadOnlySpan<char> Stem(ReadOnlySpan<char> word, Span<char> output)
        {
            if (output.Length < word.Length) throw new ArgumentException("Output buffer must be large enough to contain the entire word.");

            ToLowerInvariant(word, output);

            var wordSpan = output.Slice(0, word.Length);
            StemInternal(ref wordSpan);
            return wordSpan;
        }

        private void StemInternal(ref Span<char> word)
        {
            if (word.Length <= 2)
            {
                return;
            }

            TrimStartingApostrophe(ref word);

            if (_exceptions.TryGetValue(word, out string excpt))
            {
                excpt.AsSpan().CopyTo(word);
                word = word.Slice(0, excpt.Length);
                return;
            }

            MarkYsAsConsonants(word);

            var r1 = GetRegion1(word);
            var r2 = GetRegion(word, r1);

            Step0RemoveSPluralSuffix(ref word);
            Step1ARemoveOtherSPluralSuffixes(ref word);

            if (_exceptionsPart2.Contains(word))
            {
                ToLowerInvariant(word);
                return;
            }

            Step1BRemoveLySuffixes(ref word, r1);
            Step1CReplaceSuffixYWithIIfPreceededWithConsonant(word);
            Step2ReplaceSuffixes(ref word, r1);
            Step3ReplaceSuffixes(ref word, r1, r2);
            Step4RemoveSomeSuffixesInR2(ref word, r2);
            Step5RemoveEorLSuffixes(ref word, r1, r2);
            ToLowerInvariant(word);
        }

        private static bool SuffixInR1(ReadOnlySpan<char> word, int r1, string suffix)
        {
            return r1 <= word.Length - suffix.Length;
        }

        private static bool SuffixInR2(ReadOnlySpan<char> word, int r2, string suffix)
        {
            return r2 <= word.Length - suffix.Length;
        }

        private static Span<char> RemoveSuffix(Span<char> word, ReadOnlySpan<char> oldSuffix)
        {
            return word.Slice(0, word.Length - oldSuffix.Length);
        }

        private static void ReplaceWithShorterSuffix(ref Span<char> word, ReadOnlySpan<char> oldSuffix, ReadOnlySpan<char> newSuffix)
        {
            Debug.Assert(newSuffix.Length <= oldSuffix.Length);

            var lengthWithoutSuffix = word.Length - oldSuffix.Length;
            word = word.Slice(0, lengthWithoutSuffix + newSuffix.Length);

            newSuffix.CopyTo(word.Slice(lengthWithoutSuffix));
        }

        private static bool TryReplaceWithShorterSuffix(ref Span<char> word, ReadOnlySpan<char> oldSuffix, ReadOnlySpan<char> newSuffix)
        {
            if (MemoryExtensions.Contains(word, oldSuffix, StringComparison.Ordinal))
            {
                ReplaceWithShorterSuffix(ref word, oldSuffix, newSuffix);
                return true;
            }
            return false;
        }

        private static bool AnyVowels(ReadOnlySpan<char> chars)
        {
            foreach (var c in chars)
            {
                if (Letters.IsVowel(c)) return true;
            }
            return false;
        }

        /// <summary>
        /// The English stemmer treats apostrophe as a letter, removing it from the beginning of a word, where it might have stood for an opening quote, from the end of the word, where it might have stood for a closing quote, or been an apostrophe following s.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static void TrimStartingApostrophe(ref Span<char> word)
        {
            if (word[0] == '\'')
            {
                word = word.Slice(1);
            }
        }

        /// <summary>
        /// R1 is the region after the first non-vowel following a vowel, or the end of the word if there is no such non-vowel. 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public int GetRegion1(ReadOnlySpan<char> word)
        {
            // Exceptional forms
            if (_exceptionsRegion1.TryFindLongestPrefix(word, out var except))
            {
                return except.Length;
            }
            return GetRegion(word, 0);
        }

        /// <summary>
        /// R2 is the region after the first non-vowel following a vowel in R1, or the end of the word if there is no such non-vowel. 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public int GetRegion2(ReadOnlySpan<char> word)
        {
            var r1 = GetRegion1(word);
            return GetRegion(word, r1);
        }

        private int GetRegion(ReadOnlySpan<char> word, int begin)
        {
            var i = begin;

            for (; i < word.Length; i++)
            {
                if (Letters.IsVowel(word[i]))
                {
                    break;
                }
            }

            for (; i < word.Length; i++)
            {
                if (Letters.IsConsonant(word[i]))
                {
                    return i + 1;
                }
            }

            return word.Length;
        }

        /// <summary>
        /// Define a short syllable in a word as either (a) a vowel followed 
        /// by a non-vowel other than w, x or Y and preceded by a non-vowel, 
        /// or * (b) a vowel at the beginning of the word followed by a non-vowel. 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        internal static bool EndsInShortSyllable(ReadOnlySpan<char> word)
        {
            if (word.Length < 2)
            {
                return false;
            }

            // a vowel at the beginning of the word followed by a non-vowel
            if (word.Length == 2)
            {
                return Letters.IsVowel(word[0]) && Letters.IsConsonant(word[1]);
            }

            return Letters.IsVowel(word[word.Length - 2])
                   && Letters.IsConsonant(word[word.Length - 1])
                   && !_nonShortConsonants.Contains(word[word.Length - 1])
                   && Letters.IsConsonant(word[word.Length - 3]);
        }

        /// <summary>
        /// A word is called short if it ends in a short syllable, and if R1 is null.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        internal bool IsShortWord(ReadOnlySpan<char> word)
        {
            return EndsInShortSyllable(word) && GetRegion1(word) == word.Length;
        }

        /// <summary>
        /// Set initial y, or y after a vowel, to Y
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        internal static void MarkYsAsConsonants(Span<char> word)
        {
            for (var i = 0; i < word.Length; i++)
            {
                if (i == 0)
                {
                    if (word[i] == 'y')
                    {
                        word[i] = 'Y';
                    }
                }
                else if (word[i] == 'y' && Letters.IsVowel(word[i - 1]))
                {
                    word[i] = 'Y';
                }
            }
        }

        private static readonly EndsWithContainer step0suffixes = new EndsWithContainer("'s'", "'s", "'");
        internal static void Step0RemoveSPluralSuffix(ref Span<char> word)
        {
            if (step0suffixes.TryFindLongestSuffix(word, out var suffix))
            {
                word = RemoveSuffix(word, suffix.AsSpan());
            }
        }

        internal static void Step1ARemoveOtherSPluralSuffixes(ref Span<char> word)
        {
            var last = word[word.Length - 1];

            if ('s' != last && 'd' != last) return;
            if (EndsWith(word, "sses"))
            {
                ReplaceWithShorterSuffix(ref word, "sses".AsSpan(), "ss".AsSpan());
                return;
            }
            if (EndsWith(word, "ied") || EndsWith(word, "ies"))
            {
                // In both cases we're merely shortening the word, so reuse the buffer.
                if (word.Length > 4)
                {
                    // Trim 'ed'/'es'.
                    word = word.Slice(0, word.Length - 2);
                    return;
                }
                // Trim 'd'/'s'.
                word = word.Slice(0, word.Length - 1);
                return;
            }
            if (EndsWith(word, "us") || EndsWith(word, "ss"))
            {
                return;
            }
            if (EndsWith(word, "s"))
            {
                if (word.Length < 3)
                {
                    return;
                }

                // Skip both the last letter ('s') and the letter before that
                foreach (var c in word.Slice(0, word.Length - 2))
                {
                    if (Letters.IsVowel(c))
                    {
                        word = word.Slice(0, word.Length - 1);
                        return;
                    }
                }
            }
        }

        private static readonly EndsWithContainer step1Bsuffixes1 = new EndsWithContainer("eedly", "eed");
        private static readonly EndsWithContainer step1Bsuffixes2 = new EndsWithContainer("ed", "edly", "ing", "ingly");
        private static readonly EndsWithContainer step1Bsuffixes3 = new EndsWithContainer("at", "bl", "iz");
        internal void Step1BRemoveLySuffixes(ref Span<char> word, int r1)
        {
            if (step1Bsuffixes1.TryFindLongestSuffix(word, out var suffix))
            {
                if (SuffixInR1(word, r1, suffix))
                {
                    ReplaceWithShorterSuffix(ref word, suffix.AsSpan(), "ee".AsSpan());
                }
                return;
            }

            if (step1Bsuffixes2.TryFindLongestSuffix(word, out suffix))
            {
                var trunc = RemoveSuffix(word, suffix.AsSpan());
                if (AnyVowels(trunc))
                {
                    if (step1Bsuffixes3.EndsWithAny(trunc))
                    {
                        ReplaceWithShorterSuffix(ref word, suffix.AsSpan(), "e".AsSpan());
                        return;
                    }
                    if (_doublesEndsWith.EndsWithAny(trunc))
                    {
                        word = trunc.Slice(0, trunc.Length - 1);
                        return;
                    }
                    if (IsShortWord(trunc))
                    {
                        ReplaceWithShorterSuffix(ref word, suffix.AsSpan(), "e".AsSpan());
                        return;
                    }
                    word = trunc;
                }
            }
        }

        internal static void Step1CReplaceSuffixYWithIIfPreceededWithConsonant(Span<char> word)
        {
            var last = word[word.Length - 1];
            if ((last == 'y' || last == 'Y')
                && word.Length > 2
                && Letters.IsConsonant(word[word.Length - 2]))
            {
                // Replacing last character. Buffer is large enough, so do it in-place.
                word[word.Length - 1] = 'i';
            }
        }

        private static readonly EndsWithContainer step2Suffixes = new EndsWithContainer(
            ("ization", "ize"),
            ("ational", "ate"),
            ("ousness", "ous"),
            ("iveness", "ive"),
            ("fulness", "ful"),
            ("tional", "tion"),
            ("lessli", "less"),
            ("biliti", "ble"),
            ("entli", "ent"),
            ("ation", "ate"),
            ("alism", "al"),
            ("aliti", "al"),
            ("fulli", "ful"),
            ("ousli", "ous"),
            ("iviti", "ive"),
            ("enci", "ence"),
            ("anci", "ance"),
            ("abli", "able"),
            ("izer", "ize"),
            ("ator", "ate"),
            ("alli", "al"),
            ("bli", "ble"));

        private static bool EndsWith(ReadOnlySpan<char> word, string prefix)
        {
            var length = word.Length;

            if (length < prefix.Length) return false;
            switch (prefix.Length)
            {
                case 5:
                    return word[length - 1] == prefix[4] &&
                        word[length - 2] == prefix[3] &&
                        word[length - 3] == prefix[2] &&
                        word[length - 4] == prefix[1] &&
                        word[length - 5] == prefix[0];
                case 4:
                    return word[length - 1] == prefix[3] &&
                        word[length - 2] == prefix[2] &&
                        word[length - 3] == prefix[1] &&
                        word[length - 4] == prefix[0];
                case 3:
                    return word[length - 1] == prefix[2] &&
                        word[length - 2] == prefix[1] &&
                        word[length - 3] == prefix[0];
                case 2:
                    return word[length - 1] == prefix[1] &&
                        word[length - 2] == prefix[0];
                case 1:
                    return word[length - 1] == prefix[0];
                default:
                    return word.EndsWith(prefix.AsSpan());
            }
        }

        internal static void Step2ReplaceSuffixes(ref Span<char> word, int r1)
        {
            if (step2Suffixes.TryFindLongestSuffixAndValue(word, out var suffix, out var value))
            {
                if (SuffixInR1(word, r1, suffix))
                {
                    TryReplaceWithShorterSuffix(ref word, suffix.AsSpan(), value.AsSpan());
                }
                return;
            }

            if (EndsWith(word, "ogi"))
            {
                if (SuffixInR1(word, r1, "ogi")
                   && word[word.Length - 4] == 'l')
                {
                    ReplaceWithShorterSuffix(ref word, "ogi".AsSpan(), "og".AsSpan());
                    return;
                }
            }
            else if (EndsWith(word, "li") & SuffixInR1(word, r1, "li"))
            {
                if (_liEndings.Contains(word[word.Length - 3]))
                {
                    word = RemoveSuffix(word, "li".AsSpan());
                    return;
                }
            }
        }

        private static readonly EndsWithContainer step3suffixes = new EndsWithContainer(
            ("ational", "ate"),
            ("tional", "tion"),
            ("alize", "al"),
            ("icate", "ic"),
            ("iciti", "ic"),
            ("ical", "ic"),
            ("ful", null),
            ("ness", null)
        );
        internal static void Step3ReplaceSuffixes(ref Span<char> word, int r1, int r2)
        {
            if (step3suffixes.TryFindLongestSuffixAndValue(word, out var suffix, out var value))
            {
                if (SuffixInR1(word, r1, suffix)
                    && TryReplaceWithShorterSuffix(ref word, suffix.AsSpan(), value.AsSpan()))
                {
                    return;
                }
            }

            if (EndsWith(word, "ative"))
            {
                if (SuffixInR1(word, r1, "ative") && SuffixInR2(word, r2, "ative"))
                {
                    word = RemoveSuffix(word, "ative".AsSpan());
                    return;
                }
            }
        }

        private static readonly EndsWithContainer step4Suffixes = new EndsWithContainer(
            "al", "ance", "ence", "er", "ic", "able", "ible", "ant",
            "ement", "ment", "ent", "ism", "ate", "iti", "ous",
            "ive", "ize");
        internal static void Step4RemoveSomeSuffixesInR2(ref Span<char> word, int r2)
        {
            if (step4Suffixes.TryFindLongestSuffix(word, out var suffix))
            {
                if (SuffixInR2(word, r2, suffix))
                {
                    word = RemoveSuffix(word, suffix.AsSpan());
                }
                return;
            }

            if (EndsWith(word, "ion") &&
                SuffixInR2(word, r2, "ion"))
            {
                var c = word[word.Length - 4];
                if (c == 's' || c == 't')
                {
                    word = RemoveSuffix(word, "ion".AsSpan());
                }
            }
        }

        internal static void Step5RemoveEorLSuffixes(ref Span<char> word, int r1, int r2)
        {
            var last = word[word.Length - 1];

            if (last == 'e')
            {
                if (SuffixInR2(word, r2, "e"))
                {
                    word = RemoveSuffix(word, "e".AsSpan());
                    return;
                }
                if (SuffixInR1(word, r1, "e"))
                {
                    var temp = RemoveSuffix(word, "e".AsSpan());
                    if (!EndsInShortSyllable(temp))
                    {
                        word = temp;
                        return;
                    }
                }
            }
            else if (last == 'l' &&
                SuffixInR2(word, r2, "l") &&
                word.Length > 1 &&
                word[word.Length - 2] == 'l')
            {
                word = RemoveSuffix(word, "l".AsSpan());
            }
        }

    }
}
