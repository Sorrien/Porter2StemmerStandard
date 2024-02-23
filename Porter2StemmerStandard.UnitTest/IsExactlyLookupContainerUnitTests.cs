using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace Porter2StemmerStandard.UnitTest
{
    [TestFixture]
    public class IsExactlyLookupContainerUnitTests
    {
        [Test]
        public void TryGetValue_FindTheValue()
        {
            var target = new IsExactlyLookupContainer(
                ( "abc", "aaa" ),
                ( "xyz", "zzz" )
            );

            var actual = target.TryGetValue("abc".AsSpan(), out var actualValue);

            Assert.True(actual);

            Assert.AreEqual("aaa", actualValue);
        }

        [Test]
        [TestCase("ab")]
        [TestCase("abcd")]
        [TestCase("qwer")]
        public void TryGetValue_NotFound(string word)
        {
            var target = new IsExactlyLookupContainer(
                ( "abc", "aaa" ),
                ( "xyz", "zzz" )
            );

            var actual = target.TryGetValue(word.AsSpan(), out var actualValue);

            Assert.IsFalse(actual);
            Assert.IsNull(actualValue);
        }
    }
}
