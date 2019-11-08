using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Porter2StemmerStandard.UnitTest
{
    [TestFixture]
    public class EndsWithContainerUnitTests
    {
        [TestCase("national", "ate", "tion")]
        [TestCase("notional", "tion")]
        [TestCase("finalize", "al")]
        [TestCase("delicate", "ic")]
        public void Check(string word, params string[] expectedValues)
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

            var matches = target.Check(word).Select(m => m.Value).ToList();

            Assert.AreEqual(expectedValues, matches);
        }

        [Test]
        public void Check_WantLongestFirst()
        {
            var target = new EndsWithContainer(new[] { "'s'", "'s", "'" });

            var matches = target.Check("foo's'").Select(m => m.Suffix).ToList();

            Assert.AreEqual(new[] { "'s'", "'" }, matches);
        }
    }
}
