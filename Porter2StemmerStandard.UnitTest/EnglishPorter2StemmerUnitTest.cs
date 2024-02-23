﻿using NUnit.Framework;
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
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingTrap_IsCountedAsShort()
        {
            // Arrange
            const string word = "trap";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingEntrap_IsCountedAsShort()
        {
            // Arrange
            const string word = "entrap";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingOw_IsCountedAsShort()
        {
            // Arrange
            const string word = "ow";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingOn_IsCountedAsShort()
        {
            // Arrange
            const string word = "on";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingAt_IsCountedAsShort()
        {
            // Arrange
            const string word = "at";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsTrue(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingUproot_IsNotCountedAsShort()
        {
            // Arrange
            const string word = "uproot";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingBestow_IsCountedAsShort()
        {
            // Arrange
            const string word = "bestow";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word.AsSpan());

            // Assert
            Assert.IsFalse(actual);
        }

        [Test]
        public void EndInShortSyllable_TestingDisturb_IsCountedAsShort()
        {
            // Arrange
            const string word = "disturb";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.EndsInShortSyllable(word.AsSpan());

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
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.AreEqual("Youth", actual);
        }

        [Test]
        public void MarkVowelsAsConsonants_WithYAfterVowel_MarksYAsConsonant()
        {
            const string word = "boy";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.AreEqual("boY", actual);
        }

        [Test]
        public void MarkVowelsAsConsonants_WithYBetweenTwoVowels_MarksYAsConsonant()
        {
            const string word = "boyish";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.AreEqual("boYish", actual);
        }

        [Test]
        public void MarkVowelsAsConsonants_WithYAfterConsonant_DoesNotMarkYAsConsonant()
        {
            const string word = "fly";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.AreEqual("fly", actual);
        }

        [Test]
        public void MarkVowelsAsConsonants_WithVowelOnlyFollowingY_DoesNotMarkYAsConsonant()
        {
            const string word = "flying";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.AreEqual("flying", actual);
        }

        [Test]
        public void MarkVowelsAsConsonants_WithNoVowelsButY_DoesNotMarkAnyYAsConsonant()
        {
            const string word = "syzygy";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.AreEqual("syzygy", actual);
        }

        [Test]
        public void MarkVowelsAsConsonants_WithDoubledY_MarksFirstButNotSecondYAsConsonant()
        {
            const string word = "sayyid";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.MarkYsAsConsonants(word);

            // Assert
            Assert.AreEqual("saYyid", actual);
        }

        #endregion

        #region Remove S Plural Suffix

        [Test]
        public void RemoveSPluralSuffix_WithWordEndingInApostrophe_RemovesSuffix()
        {
            const string word = "holy'";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step0RemoveSPluralSuffix(word);

            // Assert
            Assert.AreEqual("holy", actual);
        }

        [Test]
        public void RemoveSPluralSuffix_WithWordEndingInApostropheS_RemovesSuffix()
        {
            const string word = "holy's";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step0RemoveSPluralSuffix(word);

            // Assert
            Assert.AreEqual("holy", actual);
        }

        [Test]
        public void RemoveSPluralSuffix_WithWordEndingInApostropheSApostrophe_RemovesSuffix()
        {
            const string word = "holy's'";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step0RemoveSPluralSuffix(word);

            // Assert
            Assert.AreEqual("holy", actual);
        }

        #endregion

        #region Trim sses, ied, ies, and s

        [Test]
        public void RemoveOtherSPluralSuffix_WithWordEndingInSses_ReplaceWithSs()
        {
            const string word = "assesses";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.AreEqual("assess", actual);
        }

        [Test]
        public void RemoveOtherSPluralSuffix_WithLongWordEndingInIes_ReplaceWithI()
        {
            const string word = "cries";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.AreEqual("cri", actual);
        }

        [Test]
        public void RemoveOtherSPluralSuffix_WithShortWordEndingInIes_ReplaceWithIe()
        {
            const string word = "ties";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.AreEqual("tie", actual);
        }

        [Test]
        public void RemoveOtherSPluralSuffix_WithLongWordEndingInIed_ReplaceWithI()
        {
            const string word = "cried";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.AreEqual("cri", actual);
        }

        [Test]
        public void RemoveOtherSPluralSuffix_WithShortWordEndingInIed_ReplaceWithIe()
        {
            const string word = "tied";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.AreEqual("tie", actual);
        }

        [Test]
        public void RemoveOtherSPluralSuffix_EndingInSAndContainingAVowelRightBefore_LeavesTheS()
        {
            const string word = "gas";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.AreEqual("gas", actual);
        }

        [Test]
        public void RemoveOtherSPluralSuffix_EndingInSAndContainingAVowelEarlierInWord_DeletesTheS()
        {
            const string word = "gaps";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.AreEqual("gap", actual);
        }

        [Test]
        public void RemoveOtherSPluralSuffix_EndingInSAndContainingAVowelRightBeforeAndEarlierInWord_DeletesTheS()
        {
            const string word = "kiwis";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.AreEqual("kiwi", actual);
        }

        [Test]
        public void RemoveOtherSPluralSuffix_EndingInSs_LeavesWordAlone()
        {
            const string word = "assess";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.AreEqual("assess", actual);
        }

        [Test]
        public void RemoveOtherSPluralSuffix_EndingInUs_LeavesWordAlone()
        {
            const string word = "consensus";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1ARemoveOtherSPluralSuffixes(word);

            // Assert
            Assert.AreEqual("consensus", actual);
        }

        #endregion

        #region Ly endings

        [Test]
        public void RemoveLySuffixes_EndingInEedlyAndInR1_ReplacesSuffixWithEe()
        {
            const string word = "inbreedly";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word.AsSpan()));

            // Assert
            Assert.AreEqual("inbree", actual);
        }

        [Test]
        public void RemoveLySuffixes_EndingInEedAndInR1_ReplacesSuffixWithEe()
        {
            const string word = "inbreed";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word.AsSpan()));

            // Assert
            Assert.AreEqual("inbree", actual);
        }

        [Test]
        public void RemoveLySuffixes_EndingInEdAndDoesNotContainVowel_LeavesWord()
        {
            const string word = "fred";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word.AsSpan()));

            // Assert
            Assert.AreEqual("fred", actual);
        }

        [Test]
        public void RemoveLySuffixes_EndingInEdAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriated";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word.AsSpan()));

            // Assert
            Assert.AreEqual("luxuriate", actual);
        }

        [Test]
        public void RemoveLySuffixes_EndingInEdlyAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriatedly";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word.AsSpan()));

            // Assert
            Assert.AreEqual("luxuriate", actual);
        }

        [Test]
        public void RemoveLySuffixes_EndingInIngAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriating";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word.AsSpan()));

            // Assert
            Assert.AreEqual("luxuriate", actual);
        }

        [Test]
        public void RemoveLySuffixes_EndingInInglyAndAtProceedsThat_ReplacesSuffixWithE()
        {
            const string word = "luxuriated";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word.AsSpan()));

            // Assert
            Assert.AreEqual("luxuriate", actual);
        }

        [Test]
        public void RemoveLySuffixes_EndingInIngAndDoubledLetterProceedsThat_RemovesDoubledLetter()
        {
            const string word = "hopping";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word.AsSpan()));

            // Assert
            Assert.AreEqual("hop", actual);
        }

        [Test]
        public void RemoveLySuffixes_EndingInIngAndIsShortWord_ReplacesSuffixWithE()
        {
            const string word = "hoping";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1BRemoveLySuffixes(word, stemmer.GetRegion1(word.AsSpan()));

            // Assert
            Assert.AreEqual("hope", actual);
        }

        #endregion

        #region Replace Y Suffix With I

        [Test]
        public void ReplaceYSuffix_PreceededByConsonant_ReplacesSuffixWithI()
        {
            const string word = "cry";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1CReplaceSuffixYWithIIfPreceededWithConsonant(word);

            // Assert
            Assert.AreEqual("cri", actual);
        }

        [Test]
        public void ReplaceYSuffix_PreceededByConsonantAsFirstLetterOfWord_DoesNotReplaceSuffix()
        {
            const string word = "by";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1CReplaceSuffixYWithIIfPreceededWithConsonant(word);

            // Assert
            Assert.AreEqual("by", actual);
        }

        [Test]
        public void ReplaceYSuffix_NotPreceededyConsonant_DoesNotReplaceSuffix()
        {
            const string word = "say";
            var stemmer = new EnglishPorter2Stemmer();

            // Act
            var actual = stemmer.Step1CReplaceSuffixYWithIIfPreceededWithConsonant(word);

            // Assert
            Assert.AreEqual("say", actual);
        }

        #endregion
    }
}
