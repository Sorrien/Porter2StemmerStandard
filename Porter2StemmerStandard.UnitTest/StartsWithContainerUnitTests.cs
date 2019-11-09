using NUnit.Framework;

namespace Porter2StemmerStandard.UnitTest
{
    [TestFixture]
    public class StartsWithContainerUnitTests
    {
        [Test]
        [TestCase("abcqqqqq", "abc")]
        [TestCase("abcdqqqq", "abcd")]
        [TestCase("xyz", "xyz")]
        public void TryFindLongestPrefix_FindIt(string word, string expectedPrefix)
        {
            var target = new StartsWithContainer(new []
            {
                "abc",
                "xyz",
                "abcd",
            });

            var actual = target.TryFindLongestPrefix(word, out var actualPrefix);

            Assert.IsTrue(actual);

            Assert.AreEqual(expectedPrefix, actualPrefix);
        }

        [Test]
        [TestCase("abqqqqqq")]
        [TestCase("aqcdqqqq")]
        [TestCase("xy")]
        public void TryFindLongestPrefix_DontFindIt(string word)
        {
            var target = new StartsWithContainer(new[]
            {
                "abc",
                "xyz",
                "abcd",
            });

            var actual = target.TryFindLongestPrefix(word, out var actualPrefix);

            Assert.IsFalse(actual);
        }
    }
}
