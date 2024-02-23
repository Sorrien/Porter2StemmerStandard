using NUnit.Framework;
using System;

namespace Porter2StemmerStandard.UnitTest
{
    [TestFixture]
    public class EnglishPorter2StemmerUnitTest
    {
        // ReSharper disable InconsistentNaming
        // ReSharper disable ConvertToAutoProperty
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }
        // ReSharper restore InconsistentNaming
        // ReSharper restore ConvertToAutoProperty

        #region Test Stemming

        // using a TestCaseSource with this caused the VS test runner to die
        // running all of these in one test takes < 500 ms
        [Test]
        public void Stem_WithBatchData_StemsAllWordsCorrectly()
        {
            var tests = StemBatchTestCaseSource.GetTestCaseData();

            foreach (var batchTestDataModel in tests)
            {
                // Arrange
                var stemmer = new EnglishPorter2Stemmer();
                var unstemmed = batchTestDataModel.Unstemmed;
                var expected = batchTestDataModel.Expected;

                // Act
                var stemmed = stemmer.Stem(unstemmed).Value;

                // Asssert
                Assert.AreEqual(expected, stemmed);
            }
        }

        #endregion

        #region Test Regions

        [Test]
        public void GetRegion1_WithWordContainingRegion1AndRegion2_ProvidesCorrectRangeForRegion1()
        {
            // Arrange
            const string word = "beautiful";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion1(word.AsSpan());

            // Assert
            Assert.AreEqual(5, actual);
        }

        [Test]
        public void GetRegion2_WithWordContainingRegion1AndRegion2_ProvidesCorrectRangeForRegion2()
        {
            // Arrange
            const string word = "beautiful";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion2(word.AsSpan());

            // Assert
            Assert.AreEqual(7, actual);
        }

        [Test]
        public void GetRegion1_WithWordContainingOnlyRegion1_ProvidesCorrectRangeForRegion1()
        {
            // Arrange
            const string word = "beauty";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion1(word.AsSpan());

            // Assert
            Assert.AreEqual(5, actual);
        }

        [Test]
        public void GetRegion2_WithWordContainingOnlyRegion1_ProvidesRangeWithLength0()
        {
            // Arrange
            const string word = "beauty";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion2(word.AsSpan());

            // Assert
            Assert.AreEqual(0, actual - word.Length);
        }

        [Test]
        public void GetRegion1_WithWordContainingNeitherRegion_ProvidesRangeWithLength0()
        {
            // Arrange
            const string word = "beau";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion1(word.AsSpan());

            // Assert
            Assert.AreEqual(0, actual - word.Length);
        }

        [Test]
        public void GetRegion2_WithWordContainingNeitherRegion_ProvidesRangeWithLength0()
        {
            // Arrange
            const string word = "beau";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.GetRegion2(word.AsSpan());

            // Assert
            Assert.AreEqual(0, actual - word.Length);
        }

        #endregion

        #region Test Short Syllable

        [Test]
        public void EndInShortSyllable_TestingRap_IsCountedAsShort()
        {
            // Arrange
            const string word = "rap";

            // Act
            var actual = EnglishPorter2Stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingTrap_IsCountedAsShort()
        {
            // Arrange
            const string word = "trap";

            // Act
            var actual = EnglishPorter2Stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingEntrap_IsCountedAsShort()
        {
            // Arrange
            const string word = "entrap";

            // Act
            var actual = EnglishPorter2Stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingOw_IsCountedAsShort()
        {
            // Arrange
            const string word = "ow";

            // Act
            var actual = EnglishPorter2Stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingOn_IsCountedAsShort()
        {
            // Arrange
            const string word = "on";

            // Act
            var actual = EnglishPorter2Stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingAt_IsCountedAsShort()
        {
            // Arrange
            const string word = "at";

            // Act
            var actual = EnglishPorter2Stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingUproot_IsNotCountedAsShort()
        {
            // Arrange
            const string word = "uproot";

            // Act
            var actual = EnglishPorter2Stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingBestow_IsCountedAsShort()
        {
            // Arrange
            const string word = "bestow";

            // Act
            var actual = EnglishPorter2Stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingDisturb_IsCountedAsShort()
        {
            // Arrange
            const string word = "disturb";

            // Act
            var actual = EnglishPorter2Stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsShortWord_TestingBed_IsCountedAsShort()
        {
            // Arrange
            const string word = "bed";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsShortWord_TestingShed_IsCountedAsShort()
        {
            // Arrange
            const string word = "shed";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsShortWord_TestingShred_IsCountedAsShort()
        {
            // Arrange
            const string word = "shred";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void IsShortWord_TestingBead_IsNotCountedAsShort()
        {
            // Arrange
            const string word = "bead";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word.AsSpan());

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsShortWord_TestingEmbed_IsNotCountedAsShort()
        {
            // Arrange
            const string word = "embed";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word.AsSpan());

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void IsShortWord_TestingBeds_IsNotCountedAsShort()
        {
            // Arrange
            const string word = "beds";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.IsShortWord(word.AsSpan());

            // Assert
            Assert.IsFalse(actual);
        }

        #endregion

        #region Test Marking Vowels As Consonants

        [Test]
        public void MarkVowelsAsConsonants_WithInitialY_MarksYAsConsonant()
        {
            const string word = "youth";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.MarkYsAsConsonants(buffer);

            // Assert
            Assert.AreEqual("Youth", buffer.ToString());
        }

        [Test]
        public void MarkVowelsAsConsonants_WithYAfterVowel_MarksYAsConsonant()
        {
            const string word = "boy";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.MarkYsAsConsonants(buffer);

            // Assert
            Assert.AreEqual("boY", buffer.ToString());
        }

        [Test]
        public void MarkVowelsAsConsonants_WithYBetweenTwoVowels_MarksYAsConsonant()
        {
            const string word = "boyish";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.MarkYsAsConsonants(buffer);

            // Assert
            Assert.AreEqual("boYish", buffer.ToString());
        }

        [Test]
        public void MarkVowelsAsConsonants_WithYAfterConsonant_DoesNotMarkYAsConsonant()
        {
            const string word = "fly";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.MarkYsAsConsonants(buffer);

            // Assert
            Assert.AreEqual("fly", buffer.ToString());
        }

        [Test]
        public void MarkVowelsAsConsonants_WithVowelOnlyFollowingY_DoesNotMarkYAsConsonant()
        {
            const string word = "flying";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.MarkYsAsConsonants(buffer);

            // Assert
            Assert.AreEqual("flying", buffer.ToString());
        }

        [Test]
        public void MarkVowelsAsConsonants_WithNoVowelsButY_DoesNotMarkAnyYAsConsonant()
        {
            const string word = "syzygy";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.MarkYsAsConsonants(buffer);

            // Assert
            Assert.AreEqual("syzygy", buffer.ToString());
        }

        [Test]
        public void MarkVowelsAsConsonants_WithDoubledY_MarksFirstButNotSecondYAsConsonant()
        {
            const string word = "sayyid";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.MarkYsAsConsonants(buffer);

            // Assert
            Assert.AreEqual("saYyid", buffer.ToString());
        }

        #endregion

        #region Remove S Plural Suffix

        [Test]
        public void RemoveSPluralSuffix_WithWordEndingInApostrophe_RemovesSuffix()
        {
            const string word = "holy'";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step0RemoveSPluralSuffix(ref buffer);

            // Assert
            Assert.AreEqual("holy", buffer.ToString());
        }

        [Test]
        public void RemoveSPluralSuffix_WithWordEndingInApostropheS_RemovesSuffix()
        {
            const string word = "holy's";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step0RemoveSPluralSuffix(ref buffer);

            // Assert
            Assert.AreEqual("holy", buffer.ToString());
        }

        [Test]
        public void RemoveSPluralSuffix_WithWordEndingInApostropheSApostrophe_RemovesSuffix()
        {
            const string word = "holy's'";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step0RemoveSPluralSuffix(ref buffer);

            // Assert
            Assert.AreEqual("holy", buffer.ToString());
        }

        #endregion

        #region Trim sses, ied, ies, and s

        [Test]
        public void RemoveOtherSPluralSuffix_WithWordEndingInSses_ReplaceWithSs()
        {
            const string word = "assesses";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1ARemoveOtherSPluralSuffixes(ref buffer);

            // Assert
            Assert.AreEqual("assess", buffer.ToString());
        }

        [Test]
        public void RemoveOtherSPluralSuffix_WithLongWordEndingInIes_ReplaceWithI()
        {
            const string word = "cries";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1ARemoveOtherSPluralSuffixes(ref buffer);

            // Assert
            Assert.AreEqual("cri", buffer.ToString());
        }

        [Test]
        public void RemoveOtherSPluralSuffix_WithShortWordEndingInIes_ReplaceWithIe()
        {
            const string word = "ties";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1ARemoveOtherSPluralSuffixes(ref buffer);

            // Assert
            Assert.AreEqual("tie", buffer.ToString());
        }

        [Test]
        public void RemoveOtherSPluralSuffix_WithLongWordEndingInIed_ReplaceWithI()
        {
            const string word = "cried";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1ARemoveOtherSPluralSuffixes(ref buffer);

            // Assert
            Assert.AreEqual("cri", buffer.ToString());
        }

        [Test]
        public void RemoveOtherSPluralSuffix_WithShortWordEndingInIed_ReplaceWithIe()
        {
            const string word = "tied";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1ARemoveOtherSPluralSuffixes(ref buffer);

            // Assert
            Assert.AreEqual("tie", buffer.ToString());
        }

        [Test]
        public void RemoveOtherSPluralSuffix_EndingInSAndContainingAVowelRightBefore_LeavesTheS()
        {
            const string word = "gas";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1ARemoveOtherSPluralSuffixes(ref buffer);

            // Assert
            Assert.AreEqual("gas", buffer.ToString());
        }

        [Test]
        public void RemoveOtherSPluralSuffix_EndingInSAndContainingAVowelEarlierInWord_DeletesTheS()
        {
            const string word = "gaps";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1ARemoveOtherSPluralSuffixes(ref buffer);

            // Assert
            Assert.AreEqual("gap", buffer.ToString());
        }

        [Test]
        public void RemoveOtherSPluralSuffix_EndingInSAndContainingAVowelRightBeforeAndEarlierInWord_DeletesTheS()
        {
            const string word = "kiwis";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1ARemoveOtherSPluralSuffixes(ref buffer);

            // Assert
            Assert.AreEqual("kiwi", buffer.ToString());
        }

        [Test]
        public void RemoveOtherSPluralSuffix_EndingInSs_LeavesWordAlone()
        {
            const string word = "assess";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1ARemoveOtherSPluralSuffixes(ref buffer);

            // Assert
            Assert.AreEqual("assess", buffer.ToString());
        }

        [Test]
        public void RemoveOtherSPluralSuffix_EndingInUs_LeavesWordAlone()
        {
            const string word = "consensus";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1ARemoveOtherSPluralSuffixes(ref buffer);

            // Assert
            Assert.AreEqual("consensus", buffer.ToString());
        }

        #endregion

        #region Ly endings

        [Test]
        public void RemoveLySuffixes_EndingInEedlyAndInR1_ReplacesSuffixWithEe()
        {
            const string word = "inbreedly";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var buffer = word.ToCharArray().AsSpan();
            stemmer.Step1BRemoveLySuffixes(ref buffer, stemmer.GetRegion1(buffer));

            // Assert
            Assert.AreEqual("inbree", buffer.ToString());
        }

        [Test]
        public void RemoveLySuffixes_EndingInEedAndInR1_ReplacesSuffixWithEe()
        {
            const string word = "inbreed";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var buffer = word.ToCharArray().AsSpan();
            stemmer.Step1BRemoveLySuffixes(ref buffer, stemmer.GetRegion1(buffer));

            // Assert
            Assert.AreEqual("inbree", buffer.ToString());
        }

        [Test]
        public void RemoveLySuffixes_EndingInEdAndDoesNotContainVowel_LeavesWord()
        {
            const string word = "fred";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var buffer = word.ToCharArray().AsSpan();
            stemmer.Step1BRemoveLySuffixes(ref buffer, stemmer.GetRegion1(buffer));

            // Assert
            Assert.AreEqual("fred", buffer.ToString());
        }

        [Test]
        public void RemoveLySuffixes_EndingInEdAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriated";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var buffer = word.ToCharArray().AsSpan();
            stemmer.Step1BRemoveLySuffixes(ref buffer, stemmer.GetRegion1(buffer));

            // Assert
            Assert.AreEqual("luxuriate", buffer.ToString());
        }

        [Test]
        public void RemoveLySuffixes_EndingInEdlyAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriatedly";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var buffer = word.ToCharArray().AsSpan();
            stemmer.Step1BRemoveLySuffixes(ref buffer, stemmer.GetRegion1(buffer));

            // Assert
            Assert.AreEqual("luxuriate", buffer.ToString());
        }

        [Test]
        public void RemoveLySuffixes_EndingInIngAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriating";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var buffer = word.ToCharArray().AsSpan();
            stemmer.Step1BRemoveLySuffixes(ref buffer, stemmer.GetRegion1(buffer));

            // Assert
            Assert.AreEqual("luxuriate", buffer.ToString());
        }

        [Test]
        public void RemoveLySuffixes_EndingInInglyAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriated";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var buffer = word.ToCharArray().AsSpan();
            stemmer.Step1BRemoveLySuffixes(ref buffer, stemmer.GetRegion1(buffer));

            // Assert
            Assert.AreEqual("luxuriate", buffer.ToString());
        }

        [Test]
        public void RemoveLySuffixes_EndingInIngAndDoubledLetterProceedsThat_RemovesDoubledLetter()
        {
            const string word = "hopping";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var buffer = word.ToCharArray().AsSpan();
            stemmer.Step1BRemoveLySuffixes(ref buffer, stemmer.GetRegion1(buffer));

            // Assert
            Assert.AreEqual("hop", buffer.ToString());
        }

        [Test]
        public void RemoveLySuffixes_EndingInIngAndIsShortWord_ReplacesSuffixWithE()
        {
            const string word = "hoping";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var buffer = word.ToCharArray().AsSpan();
            stemmer.Step1BRemoveLySuffixes(ref buffer, stemmer.GetRegion1(buffer));

            // Assert
            Assert.AreEqual("hope", buffer.ToString());
        }

        #endregion

        #region Replace Y Suffix With I

        [Test]
        public void ReplaceYSuffix_PreceededByConsonant_ReplacesSuffixWithI()
        {
            const string word = "cry";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1CReplaceSuffixYWithIIfPreceededWithConsonant(buffer);

            // Assert
            Assert.AreEqual("cri", buffer.ToString());
        }

        [Test]
        public void ReplaceYSuffix_PreceededByConsonantAsFirstLetterOfWord_DoesNotReplaceSuffix()
        {
            const string word = "by";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1CReplaceSuffixYWithIIfPreceededWithConsonant(buffer);

            // Assert
            Assert.AreEqual("by", buffer.ToString());
        }

        [Test]
        public void ReplaceYSuffix_NotPreceededyConsonant_DoesNotReplaceSuffix()
        {
            const string word = "say";

            // Act
            var buffer = word.ToCharArray().AsSpan();
            EnglishPorter2Stemmer.Step1CReplaceSuffixYWithIIfPreceededWithConsonant(buffer);

            // Assert
            Assert.AreEqual("say", buffer.ToString());
        }

        #endregion
    }
}
