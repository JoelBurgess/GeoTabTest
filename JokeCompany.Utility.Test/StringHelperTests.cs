using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace JokeCompany.Utility.Test
{
    [TestFixture]
    public class StringHelperTests
    {
        private const string ExtraWord = "TooLong";

        [Test]
        public void WordWrapWrapsLongText()
        {
            const int maxWidth = 50;
            const int wordWidth = 7;
            const int numberOfRows = 5;

            var rows = BuildRowsOneWordTooLong(numberOfRows, wordWidth, maxWidth); // Build rows of repeating words, but add one (which doesn't match) too many.

            var fullText = string.Join("", rows);
            var wrappedText = fullText.WordWrap(maxWidth).ToList();
            
            // Make sure that text round-trips, or else any other testing could mean false positives.
            Assert.AreEqual(fullText, string.Join(" ", wrappedText));

            // Make sure that the length isn't too long.
            var lengthTooLong = wrappedText.FirstOrDefault(line => line.Length > maxWidth);
            Assert.IsNull(lengthTooLong, $"Line length too long ({lengthTooLong?.Length}) on line '{lengthTooLong}");

            // Every row but the first should have one (and only one) wrapped word.
            var didNotWrap = wrappedText.Skip(1).FirstOrDefault(w => w.IndexOf(ExtraWord) < 0 || w.IndexOf(ExtraWord) != w.LastIndexOf(ExtraWord));
            Assert.IsNull(didNotWrap, $"Extra word(s) wrong on line '{didNotWrap}'");

            // Make sure that the length isn't too short, if not the last line.
            var lengthTooShort = wrappedText.Take(wrappedText.Count - 1).FirstOrDefault(line => maxWidth - line.Length > wordWidth);
            Assert.IsNull(lengthTooShort, $"Line length too short ({lengthTooShort?.Length}) on line '{lengthTooShort}");
        }

        [Test]
        public void WordWrapDoesNothingToShortText()
        {
            const int maxWidth = 50;

            var fullText = "This is too short to wrap!";
            var wrappedText = fullText.WordWrap(maxWidth).ToList();

            Assert.AreEqual(1, wrappedText.Count);
            Assert.AreEqual(fullText, wrappedText.Last());
        }

        [Test]
        public void WordWrapDoesNothingToWhitespaceOnly()
        {
            const int maxWidth = 50;

            var fullText = new string(' ', 500);
            var wrappedText = fullText.WordWrap(maxWidth).ToList();

            Assert.AreEqual(1, wrappedText.Count);
            Assert.AreEqual(fullText, wrappedText.Last());
        }

        [Test]
        public void WordWrapThrowsOnNull()
        {
            Assert.Throws<ArgumentNullException>(() => ((string) null).WordWrap(19));
        }

        private static List<string> BuildRowsOneWordTooLong(int numberOfRows, int wordWidth, int maxWidth)
        {
            var rows = new List<string>();

            for (var i = 0; i < numberOfRows; i++)
            {
                var newLine = new StringBuilder();
                var wordOfLen = new string('a', wordWidth);

                while (newLine.Length + wordOfLen.Length + 1 < maxWidth)
                {
                    newLine.Append(wordOfLen);
                    newLine.Append(" ");
                }

                newLine.Append(ExtraWord);

                rows.Add(newLine.ToString());
            }

            return rows;
        }
    }
}