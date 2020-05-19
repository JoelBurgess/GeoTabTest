using System;
using System.Collections.Generic;
using System.Linq;

namespace JokeCompany.Utility
{
    /// <summary>
    /// Helper methods for strings.
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Wraps string to <paramref name="maxWidth"/> width. Determines words using ASCII space.
        /// </summary>
        /// <param name="fullText">Test to wrap.</param>
        /// <param name="maxWidth">Length to wrap to.</param>
        /// <returns>Enumerable of strings of wrapped text (or in the case of all white-space, the same text unwrapped as the first string).</returns>
        public static IEnumerable<string> WordWrap(this string fullText, int maxWidth)
        {
            if (fullText == null)
            {
                throw new ArgumentNullException(nameof(fullText));
            }

            if (string.IsNullOrWhiteSpace(fullText))
            {
                // All whitespace doesn't make much sense to request wrapping on, so we just return it as-is.
                return new[] {fullText};
            }

            var words = fullText.Split(' ');

            var wrappedLines = words.Skip(1).Aggregate(words.Take(1).ToList(), (lines, nextWord) =>
            {
                // Line would overflow, go to next line.
                if (lines.Last().Length + nextWord.Length >= maxWidth)
                {
                    lines.Add(nextWord);
                }
                else // Current line has capacity for the current word.
                {
                    lines[lines.Count - 1] += " " + nextWord;
                }

                return lines;
            });

            return wrappedLines;
        }
    }
}