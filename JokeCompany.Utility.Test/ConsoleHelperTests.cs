using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

namespace JokeCompany.Utility.Test
{
    [TestFixture]
    public class ConsoleHelperTests
    {
        private static readonly List<string> _badPrompts = new List<string> {null, " ", "       "};
        private readonly List<string> _possibleListAnswers = new List<string> { "apple", "ORANGE", "baNana" };
        private readonly List<string> _sortedListAnswers = new List<string> { "apple", "baNana", "ORANGE" };
        private const string Prompt = "Test";

        [Test]
        [TestCase("y")]
        [TestCase("Y")]
        [TestCase("yes")]
        [TestCase("YES")]
        [TestCase("yEs")]
        [TestCase("YeS")]
        [TestCase("yES")]
        [TestCase("Yes")]
        public void TestValidYesResponses(string readLine)
        {
            // Try different combinations of whitespace.
            TestValidYesNoResponse(readLine, true);
            TestValidYesNoResponse($"{readLine}  ", true);
            TestValidYesNoResponse($"  {readLine}", true);
            TestValidYesNoResponse($"  {readLine}  ", true);
        }

        [Test]
        [TestCase("n")]
        [TestCase("N")]
        [TestCase("no")]
        [TestCase("NO")]
        [TestCase("nO")]
        [TestCase("No")]
        public void TestValidNoResponses(string readLine)
        {
            // Try different combinations of whitespace.
            TestValidYesNoResponse(readLine, false);
            TestValidYesNoResponse($"{readLine}  ", false);
            TestValidYesNoResponse($"  {readLine}", false);
            TestValidYesNoResponse($"  {readLine}  ", false);
        }

        [Test]
        [TestCaseSource(nameof(_badPrompts))]
        public void BadYesNoPromptsThrowsException(string prompt)
        {
            var helper = new ConsoleHelper(Encoding.UTF8);
            Assert.Throws<ArgumentException>(() => helper.IsResponseYes(prompt), "Argument exception was expected for bad prompt.");
        }

        [Test]
        public void PromptForDigitAcceptsValidDigits()
        {
            foreach (var digit in Enumerable.Range(1, 9)) // Skip test cases since we can generate this, and the individual test cases don't buy us much.
            {
                var helper = SetupMockToReturn(digit.ToString());
                
                Assert.AreEqual(digit, helper.Object.PromptForDigit(Prompt));

                helper.Verify(h => h.WriteLine($"{Prompt} (1-9)"), Times.Once());
                helper.Verify(h => h.ReadLine(), Times.Once());
            }
        }

        [Test]
        public void PromptForDigitAcceptsValidDigitsWithWhitespace()
        {
            foreach (var digit in Enumerable.Range(1, 9)) // Skip test cases since we can generate this, and the individual test cases don't buy us much.
            {
                var helper = SetupMockToReturn($"  {digit}  ");
                
                Assert.AreEqual(digit, helper.Object.PromptForDigit(Prompt));

                helper.Verify(h => h.WriteLine($"{Prompt} (1-9)"), Times.Once());
                helper.Verify(h => h.ReadLine(), Times.Once());
            }
        }

        [Test]
        public void PromptForDigitAcceptsValidDigitsAfterInvalid()
        {
            const int validAnswer = 1;

            // It's a fraction of a second to test every possible invalid byte value so let's test them all.
            var allInvalidShortDigits = Enumerable.Range(byte.MinValue, byte.MaxValue).Except(Enumerable.Range(1, 9)).ToArray();
            var allInvalidWithValidOnEnd = allInvalidShortDigits.Append(validAnswer).ToArray();
            var helper = SetupMockToReturn(allInvalidWithValidOnEnd.Select(a => a.ToString()).ToArray());
            
            Assert.AreEqual(validAnswer, helper.Object.PromptForDigit(Prompt));

            helper.Verify(h => h.WriteLine($"{Prompt} (1-9)"), Times.Exactly(allInvalidWithValidOnEnd.Length));
            helper.Verify(h => h.ReadLine(), Times.Exactly(allInvalidWithValidOnEnd.Length));
            helper.Verify(h => h.WriteLine("Invalid selection."), Times.Exactly(allInvalidShortDigits.Length));
        }

        [Test]
        [TestCase("apple")]
        [TestCase(" apple")]
        [TestCase("apple ")]
        [TestCase(" apple ")]
        [TestCase("banana")]
        [TestCase("orangE")]
        public void PromptForListSelectAcceptsValidValueByName(string selection)
        {
            var helper = SetupMockToReturn(selection);
            var selectedValue = helper.Object.PromptForListSelection(Prompt, _possibleListAnswers);

            StringAssert.AreEqualIgnoringCase(selection.Trim(), selectedValue);
            ValidateListSelectPrompts(helper, Prompt, _sortedListAnswers, Times.Once());
        }

        [Test]
        public void PromptForListSelectAcceptsValidValueByIndex()
        {
            for (var i = 1; i <= _sortedListAnswers.Count; i++) // Start at 1, since that's how PromptForListSelection displays them.
            {
                var selection = _sortedListAnswers[i - 1];
                var helper = SetupMockToReturn(i.ToString());

                var selectedValue = helper.Object.PromptForListSelection(Prompt, _possibleListAnswers);

                StringAssert.AreEqualIgnoringCase(selection.Trim(), selectedValue);
                ValidateListSelectPrompts(helper, Prompt, _sortedListAnswers, Times.Once());
            }
        }

        [Test]
        public void PromptForListSelectAcceptsValidValueByNameAfterInvalidValues()
        {
            var selection = _possibleListAnswers.First();
                
            var helper = SetupMockToReturn("bad", "worse", selection);
            var selectedValue = helper.Object.PromptForListSelection(Prompt, _possibleListAnswers);

            StringAssert.AreEqualIgnoringCase(selection.Trim(), selectedValue);
            ValidateListSelectPrompts(helper, Prompt, _sortedListAnswers, Times.Exactly(3));
            helper.Verify(h => h.WriteLine("Invalid selection."), Times.Exactly(2));
        }

        [Test]
        public void PromptForListSelectAcceptsValidValueByIndexAfterInvalidValues()
        {
            var selection = _possibleListAnswers.First();

            var helper = SetupMockToReturn("bad", "99", "1");
            var selectedValue = helper.Object.PromptForListSelection(Prompt, _possibleListAnswers);

            StringAssert.AreEqualIgnoringCase(selection.Trim(), selectedValue);
            ValidateListSelectPrompts(helper, Prompt, _sortedListAnswers, Times.Exactly(3));
            helper.Verify(h => h.WriteLine("Invalid selection."), Times.Exactly(2));
        }

        [Test]
        [TestCaseSource(nameof(_badPrompts))]
        public void PromptForListSelectThrowsWhenHeaderIsInvalid(string header)
        {
            var helper = new ConsoleHelper(Encoding.UTF8);
            Assert.Throws<ArgumentException>(() => helper.PromptForListSelection(header, _possibleListAnswers));
        }

        [Test]
        public void PromptForListSelectThrowsWhenAnswersAreNull()
        {
            var helper = new ConsoleHelper(Encoding.UTF8);
            Assert.Throws<ArgumentException>(() => helper.PromptForListSelection(Prompt, null));
        }

        [Test]
        public void PromptForListSelectThrowsWhenAnswersAreEmpty()
        {
            var helper = new ConsoleHelper(Encoding.UTF8);
            Assert.Throws<ArgumentException>(() => helper.PromptForListSelection(Prompt, new List<string>()));
        }

        [Test]
        public void PromptForListSelectAcceptsHandlesNullPossibleAnswer()
        {
            var helper = SetupMockToReturn(_possibleListAnswers.Last());
            var answersWithBadEntry = _possibleListAnswers.ToList().Append(null).ToList();
            var selectedValue = helper.Object.PromptForListSelection(Prompt, answersWithBadEntry);

            StringAssert.AreEqualIgnoringCase(selectedValue, selectedValue);
            ValidateListSelectPrompts(helper, Prompt, _sortedListAnswers, Times.Once());
        }

        [Test]
        public void PrintResultsHandlesValidTestWithoutWordWrapping()
        {
            var list = new List<string> { "Line 1",  "Line 2", "Line 3" };
            var helper = SetupMockForPrintResults(50);
            
            helper.Object.PrintResults(list);

            helper.Verify(h => h.WriteEmptyLine(), Times.Exactly(list.Count + 1)); // One extra at end 
            helper.Verify(h => h.GetWindowWidth(), Times.Once);
            foreach (var item in list)
            {
                helper.Verify(h => h.WriteLine(item), Times.Once);
            }
            helper.VerifyNoOtherCalls();
        }

        [Test]
        public void PrintResultsHandlesValidTestWithWordWrapping()
        {
            var list = new List<string>
            {
                "Line 1 Is Too Long", 
                "Line 2", 
                "Line 3"
            };
            var helper = SetupMockForPrintResults(10);

            helper.Object.PrintResults(list);

            helper.Verify(h => h.WriteEmptyLine(), Times.Exactly(list.Count + 1)); // One extra at end
            helper.Verify(h => h.GetWindowWidth(), Times.Once);
            helper.Verify(h => h.WriteLine(It.IsAny<string>()), Times.Exactly(list.Count + 1));
            helper.VerifyNoOtherCalls();
        }

        [Test]
        public void PrintResultsHandlesValidTestWithNullInList()
        {
            var list = new List<string> { "Line 1", "Line 2", "Line 3", null };
            var helper = SetupMockForPrintResults(50);

            helper.Object.PrintResults(list);

            helper.Verify(h => h.WriteEmptyLine(), Times.Exactly(list.Count)); // One less this time due to null.
            helper.Verify(h => h.GetWindowWidth(), Times.Once);
            foreach (var item in list.Where(i => i != null))
            {
                helper.Verify(h => h.WriteLine(item), Times.Once);
            }
            helper.VerifyNoOtherCalls();
        }

        [Test]
        public void PrintResultsThrowsWhenNullList()
        {
            var helper = new ConsoleHelper(Encoding.UTF8);
            Assert.Throws<ArgumentNullException>(() => helper.PrintResults(null));
        }


        private static Mock<ConsoleHelper> SetupMockForPrintResults(int windowWidth = 50)
        {
            var helper = new Mock<ConsoleHelper>(Encoding.UTF8) {CallBase = true};
            helper.Setup(h => h.WriteEmptyLine());
            helper.Setup(h => h.GetWindowWidth()).Returns(windowWidth);
            helper.Setup(h => h.WriteLine(It.IsAny<string>()));

            return helper;
        }

        private static void ValidateListSelectPrompts(Mock<ConsoleHelper> helper, string prompt, List<string> sortedAnswers, Times times)
        {
            helper.Verify(h => h.WriteLine($"{prompt}"), times);

            for (var i = 0; i < sortedAnswers.Count; i++)
            {
                var text = sortedAnswers[i];
                helper.Verify(h => h.WriteLine($"{i + 1} - {text}"), times);
            }

            helper.Verify(h => h.ReadLine(), times);
        }

        private static void TestValidYesNoResponse(string readLine, bool expected)
        {
            var helper = SetupMockToReturn(readLine);
            
            Assert.AreEqual(expected, helper.Object.IsResponseYes(Prompt));

            helper.Verify(h => h.WriteLine($"{Prompt} y/n"), Times.Once());
            helper.Verify(h => h.ReadLine(), Times.Once());
        }

        [TestCase("y", true)]
        [TestCase("n", false)]
        public void TestInvalidYesNoResponsesFollowedByValidResponse(string readLine, bool expectedResponse)
        {
            var helper = SetupMockToReturn("asd", "asdf", readLine);
            
            Assert.AreEqual(expectedResponse, helper.Object.IsResponseYes(Prompt));

            helper.Verify(h => h.WriteLine($"{Prompt} y/n"), Times.Exactly(3));
            helper.Verify(h => h.WriteLine("Invalid selection."), Times.Exactly(2));
            helper.Verify(h => h.ReadLine(), Times.Exactly(3));
        }

        private static Mock<ConsoleHelper> SetupMockToReturn(params string[] responses)
        {
            // Test concrete methods, by using mock to override console writing/reading.
            var helper = new Mock<ConsoleHelper>(Encoding.UTF8) {CallBase = true};
            helper.Setup(h => h.Clear());
            helper.Setup(h => h.WriteLine(It.IsAny<string>()));
            helper.Setup(h => h.ReadLine()).Returns(new Queue<string>(responses).Dequeue);
            
            return helper;
        }
    }
}