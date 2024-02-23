using System;
using System.Collections.Generic;
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

        public StemmedWord Stem(string word)
        {
            var original = word;
            if (word.Length <= 2)
            {
                return new StemmedWord(word, word);
            }

            word = TrimStartingApostrophe(word.ToLowerInvariant());

            if (_exceptions.TryGetValue(word, out string excpt))
            {
                return new StemmedWord(excpt, original);
            }

            word = MarkYsAsConsonants(word);

            var r1 = GetRegion1(word);
            var r2 = GetRegion(word, r1);

            word = Step0RemoveSPluralSuffix(word);
            word = Step1ARemoveOtherSPluralSuffixes(word);

            if (_exceptionsPart2.Contains(word))
            {
                return new StemmedWord(word, original);
            }

            word = Step1BRemoveLySuffixes(word, r1);
            word = Step1CReplaceSuffixYWithIIfPreceededWithConsonant(word);
            word = Step2ReplaceSuffixes(word, r1);
            word = Step3ReplaceSuffixes(word, r1, r2);
            word = Step4RemoveSomeSuffixesInR2(word, r2);
            word = Step5RemoveEorLSuffixes(word, r1, r2);

            return new StemmedWord(word.ToLowerInvariant(), original);
        }

        private static bool SuffixInR1(string word, int r1, string suffix)
        {
            return r1 <= word.Length - suffix.Length;
        }

        private bool SuffixInR2(string word, int r2, string suffix)
        {
            return r2 <= word.Length - suffix.Length;
        }

        private static string ReplaceSuffix(string word, string oldSuffix, string newSuffix = null)
        {
            if (oldSuffix != null)
            {
                word = word.Substring(0, word.Length - oldSuffix.Length);
            }

            if (newSuffix != null)
            {
                word += newSuffix;
            }
            return word;
        }

        private static bool TryReplace(string word, string oldSuffix, string newSuffix, out string final)
        {
            if (word.Contains(oldSuffix))
            {
                final = ReplaceSuffix(word, oldSuffix, newSuffix);
                return true;
            }
            final = word;
            return false;
        }

        /// <summary>
        /// The English stemmer treats apostrophe as a letter, removing it from the beginning of a word, where it might have stood for an opening quote, from the end of the word, where it might have stood for a closing quote, or been an apostrophe following s.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        private static string TrimStartingApostrophe(string word)
        {
            if (word[0] == '\'')
            {
                word = word.Substring(1);
            }
            return word;
        }

        /// <summary>
        /// R1 is the region after the first non-vowel following a vowel, or the end of the word if there is no such non-vowel. 
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public int GetRegion1(string word)
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
        public int GetRegion2(string word)
        {
            var r1 = GetRegion1(word);
            return GetRegion(word, r1);
        }

        private int GetRegion(string word, int begin)
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
        public bool EndsInShortSyllable(string word)
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
        public bool IsShortWord(string word)
        {
            return EndsInShortSyllable(word) && GetRegion1(word) == word.Length;
        }

        /// <summary>
        /// Set initial y, or y after a vowel, to Y
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public string MarkYsAsConsonants(string word)
        {
            var chars = word.ToCharArray();
            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 0)
                {
                    if (chars[i] == 'y')
                    {
                        chars[i] = 'Y';
                    }
                }
                else if (chars[i] == 'y' && Letters.IsVowel(chars[i - 1]))
                {
                    chars[i] = 'Y';
                }
            }
            return new string(chars);
        }

        private static readonly EndsWithContainer step0suffixes = new EndsWithContainer("'s'", "'s", "'");
        public string Step0RemoveSPluralSuffix(string word)
        {
            if (step0suffixes.TryFindLongestSuffix(word, out var suffix))
            {
                return ReplaceSuffix(word, suffix);
            }
            return word;
        }

        public string Step1ARemoveOtherSPluralSuffixes(string word)
        {
            var last = word[word.Length - 1];

            if ('s' != last && 'd' != last) return word;
            if (EndsWith(word, "sses"))
            {
                return ReplaceSuffix(word, "sses", "ss");
            }
            if (EndsWith(word, "ied") || EndsWith(word, "ies"))
            {
                var restOfWord = word.Substring(0, word.Length - 3);
                if (word.Length > 4)
                {
                    return restOfWord + "i";
                }
                return restOfWord + "ie";
            }
            if (EndsWith(word, "us") || EndsWith(word, "ss"))
            {
                return word;
            }
            if (EndsWith(word, "s"))
            {
                if (word.Length < 3)
                {
                    return word;
                }

                // Skip both the last letter ('s') and the letter before that
                for (var i = 0; i < word.Length - 2; i++)
                {
                    if (Letters.IsVowel(word[i]))
                    {
                        return word.Substring(0, word.Length - 1);
                    }
                }
            }
            return word;
        }

        private static readonly EndsWithContainer step1Bsuffixes1 = new EndsWithContainer("eedly", "eed");
        private static readonly EndsWithContainer step1Bsuffixes2 = new EndsWithContainer("ed", "edly", "ing", "ingly");
        private static readonly EndsWithContainer step1Bsuffixes3 = new EndsWithContainer("at", "bl", "iz");
        public string Step1BRemoveLySuffixes(string word, int r1)
        {
            if (step1Bsuffixes1.TryFindLongestSuffix(word, out var suffix))
            {
                if (SuffixInR1(word, r1, suffix))
                {
                    return ReplaceSuffix(word, suffix, "ee");
                }
                return word;
            }

            if (step1Bsuffixes2.TryFindLongestSuffix(word, out suffix))
            {
                var trunc = ReplaceSuffix(word, suffix);//word.Substring(0, word.Length - suffix.Length);
                if (trunc.Any(Letters.IsVowel))
                {
                    if (step1Bsuffixes3.EndsWithAny(trunc))
                    {
                        return trunc + "e";
                    }
                    if (_doublesEndsWith.EndsWithAny(trunc))
                    {
                        return trunc.Substring(0, trunc.Length - 1);
                    }
                    if (IsShortWord(trunc))
                    {
                        return trunc + "e";
                    }
                    return trunc;
                }
                return word;
            }

            return word;
        }

        public string Step1CReplaceSuffixYWithIIfPreceededWithConsonant(string word)
        {
            var last = word[word.Length - 1];
            if ((last == 'y' || last == 'Y')
                && word.Length > 2
                && Letters.IsConsonant(word[word.Length - 2]))
            {
                return word.Substring(0, word.Length - 1) + "i";
            }
            return word;
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

        private static bool EndsWith(string word, string prefix)
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
                    return word.EndsWith(prefix);
            }
        }

        public string Step2ReplaceSuffixes(string word, int r1)
        {
            if (step2Suffixes.TryFindLongestSuffixAndValue(word, out var suffix, out var value))
            {
                if (SuffixInR1(word, r1, suffix)
                    && TryReplace(word, suffix, value, out var final))
                {
                    return final;
                }
                return word;
            }

            if (EndsWith(word, "ogi"))
            {
                if (SuffixInR1(word, r1, "ogi")
                   && word[word.Length - 4] == 'l')
                {
                    return ReplaceSuffix(word, "ogi", "og");
                }
            }
            else if (EndsWith(word, "li") & SuffixInR1(word, r1, "li"))
            {
                if (_liEndings.Contains(word[word.Length - 3]))
                {
                    return ReplaceSuffix(word, "li");
                }
            }

            return word;
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
        public string Step3ReplaceSuffixes(string word, int r1, int r2)
        {
            if (step3suffixes.TryFindLongestSuffixAndValue(word, out var suffix, out var value))
            {
                if (SuffixInR1(word, r1, suffix)
                    && TryReplace(word, suffix, value, out string final))
                {
                    return final;
                }
            }

            if (EndsWith(word, "ative"))
            {
                if (SuffixInR1(word, r1, "ative") && SuffixInR2(word, r2, "ative"))
                {
                    return ReplaceSuffix(word, "ative", null);
                }
            }

            return word;
        }

        private static readonly EndsWithContainer step4Suffixes = new EndsWithContainer(
            "al", "ance", "ence", "er", "ic", "able", "ible", "ant",
            "ement", "ment", "ent", "ism", "ate", "iti", "ous",
            "ive", "ize");
        public string Step4RemoveSomeSuffixesInR2(string word, int r2)
        {
            if (step4Suffixes.TryFindLongestSuffix(word, out var suffix))
            {
                if (SuffixInR2(word, r2, suffix))
                {
                    return ReplaceSuffix(word, suffix);
                }
                return word;
            }

            if (EndsWith(word, "ion") &&
                SuffixInR2(word, r2, "ion") &&
                "st".Contains(word[word.Length - 4]))
            {
                return ReplaceSuffix(word, "ion");
            }
            return word;
        }

        public string Step5RemoveEorLSuffixes(string word, int r1, int r2)
        {
            var last = word[word.Length - 1];

            if (last == 'e')
            {
                if (SuffixInR2(word, r2, "e") ||
                    (SuffixInR1(word, r1, "e") &&
                        !EndsInShortSyllable(ReplaceSuffix(word, "e"))))
                {
                    return ReplaceSuffix(word, "e");
                }
            }
            else if (last == 'l' &&
                SuffixInR2(word, r2, "l") &&
                word.Length > 1 &&
                word[word.Length - 2] == 'l')
            {
                return ReplaceSuffix(word, "l");
            }

            return word;
        }
    }
}
