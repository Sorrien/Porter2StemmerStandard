using System;
using NUnit.Framework;

namespace Porter2StemmerStandard.UnitTest
{
    [TestFixture]
    public class IsExactlyContainerUnitTests
    {
        [Test]
        public void Contains_ItDoes()
        {
            var target = new IsExactlyContainer(
                "abc",
                "xyz"
            );

            var actual = target.Contains("abc".AsSpan());

            Assert.True(actual);
        }

        [Test]
        [TestCase("ab")]
        [TestCase("abcd")]
        [TestCase("qwer")]
        public void Contains_ItDoesNot(string word)
        {
            var target = new IsExactlyContainer(
                "abc",
                "xyz"
            );

            var actual = target.Contains(word.AsSpan());

            Assert.False(actual);
        }
    }
}
