using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Porter2StemmerStandard.UnitTest
{
    [TestFixture]
    public class EndsWithContainerUnitTests
    {
        [TestCase("national", "ate")]
        [TestCase("notional", "tion")]
        [TestCase("finalize", "al")]
        [TestCase("delicate", "ic")]
        public void Check(string word, string expectedValue)
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

    }
}
