using NUnit.Framework;
using System.Collections.Generic;

namespace Porter2StemmerStandard.UnitTest
{
    [TestFixture]
    public class EndsWithContainerUnitTests
    {
        [TestCase("national", "ate")]
        [TestCase("notional", "tion")]
        [TestCase("finalize", "al")]
        [TestCase("delicate", "ic")]
        public void TryFindLongestSuffixAndValue_FindIt(string word, string expectedValue)
        {
            var target = new EndsWithContainer(new Dictionary<string, string>
            {
                {"ational", "ate"},
                {"tional", "tion"},
                {"alize", "al"},
                {"icate", "ic"},
                {"iciti", "ic"},
                {"ical", "ic"},
            });

            var actual = target.TryFindLongestSuffixAndValue(word, out var _, out var value);

            Assert.IsTrue(actual);

            Assert.AreEqual(expectedValue, value);
        }

        [TestCase("bional")]
        [TestCase("ional")]
        [TestCase("abcd")]
        [TestCase("cal")]
        public void TryFindLongestSuffixAndValue_DontFindIt(string word)
        {
            var target = new EndsWithContainer(new Dictionary<string, string>
            {
                {"ational", "ate"},
                {"tional", "tion"},
                {"alize", "al"},
                {"icate", "ic"},
                {"iciti", "ic"},
                {"ical", "ic"},
            });

            var actual = target.TryFindLongestSuffixAndValue(word, out var suffix, out var value);

            Assert.IsFalse(actual);
            Assert.IsNull(suffix);
            Assert.IsNull(value);
        }

        [TestCase("national", true)]
        [TestCase("tional", true)]
        [TestCase("notional", true)]
        [TestCase("finalize", true)]
        [TestCase("delicate", true)]
        [TestCase("abc", false)]
        [TestCase("bional", false)]
        public void EndsWithAny(string word, bool expected)
        {
            var target = new EndsWithContainer(new[]
            {
                "ational",
                "tional",
                "alize",
                "icate",
                "iciti",
                "ical",
            });

            var actual = target.EndsWithAny(word);

            Assert.AreEqual(expected, actual);
        }
    }
}
