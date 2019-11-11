using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Porter2StemmerStandard.UnitTest
{
    [TestFixture]
    public class LettersUnitTests
    {
        [Test]
        [TestCase('a', true)]
        [TestCase('e', true)]
        [TestCase('i', true)]
        [TestCase('o', true)]
        [TestCase('u', true)]
        [TestCase('y', true)]
        [TestCase('Y', false)]
        [TestCase('b', false)]
        [TestCase('z', false)]
        [TestCase('{', false)]
        [TestCase('€', false)]
        public void IsVowel(char input, bool expected)
        {
            var actual = Letters.IsVowel(input);

            Assert.AreEqual(expected, actual);
        }
    }
}
