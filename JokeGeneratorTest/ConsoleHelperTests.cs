using System.Collections.Generic;
using System.Text;
using JokeGenerator;
using Moq;
using NUnit.Framework;

namespace JokeGeneratorTest
{
    [TestFixture]
    public class ConsoleHelperTests
    {
        [Test]
        public void Test()
        {
            Assert.Fail();
        }

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
            TestValidResponse(readLine, true);
            TestValidResponse($"{readLine}  ", true);
            TestValidResponse($"  {readLine}", true);
            TestValidResponse($"  {readLine}  ", true);
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
            TestValidResponse(readLine, false);
            TestValidResponse($"{readLine}  ", false);
            TestValidResponse($"  {readLine}", false);
            TestValidResponse($"  {readLine}  ", false);
        }

        private static void TestValidResponse(string readLine, bool expected)
        {
            var helper = SetupMock(readLine);
            const string prompt = "Test";

            Assert.AreEqual(expected, helper.Object.IsResponseYes(prompt));

            helper.Verify(h => h.WriteLine($"{prompt} y/n"), Times.Once());
            helper.Verify(h => h.ReadLine(), Times.Once());
        }

        [TestCase("y", true)]
        [TestCase("n", false)]
        public void TestInvalidYesNoResponsesFollowedByValidResponse(string readLine, bool expectedResponse)
        {
            var helper = SetupMock("asd", "asdf", readLine);
            const string prompt = "Test";

            Assert.AreEqual(expectedResponse, helper.Object.IsResponseYes(prompt));

            helper.Verify(h => h.WriteLine($"{prompt} y/n"), Times.Exactly(3));
            helper.Verify(h => h.WriteLine("Invalid selection."), Times.Exactly(2));
            helper.Verify(h => h.ReadLine(), Times.Exactly(3));
        }

        private static Mock<ConsoleHelper> SetupMock(params string[] responses)
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