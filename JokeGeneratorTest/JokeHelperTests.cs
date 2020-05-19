using System.Linq;
using JokeCompany.ExternalFeeds.ChuckNorris;
using JokeCompany.ExternalFeeds.RandomName;
using JokeGenerator;
using Moq;
using NUnit.Framework;

namespace JokeGeneratorTest
{
    [TestFixture]
    public class JokeHelperTests
    {
        private const string JokeTest = "This is the joke.";
        private const string JokeTestWithCategory = JokeTest + " With Category";
        private const string ReplacementName = "First Last";

        [Test]
        public void GetRandomJokesWorksWithOneQuestionAndNoCategoryOrName()
        {
            var helper = GetJokeHelperWithMockFeed(null);
            var jokes = helper.GetRandomJokes(1).ToList();
            
            Assert.AreEqual(1, jokes.Count);
            Assert.IsTrue(jokes.Single() == JokeTest);
        }

        [Test]
        public void GetRandomJokesWorksWithManyQuestionsAndNoCategoryOrName()
        {
            var helper = GetJokeHelperWithMockFeed(null);
            var jokes = helper.GetRandomJokes(5).ToList();

            Assert.AreEqual(5, jokes.Count);
            Assert.IsTrue(jokes.All(j => j == JokeTest));
        }

        [Test]
        public void GetRandomJokesWorksWithCategory()
        {
            var helper = GetJokeHelperWithMockFeed(null);
            var jokes = helper.GetRandomJokes(1, "category!").ToList();

            Assert.AreEqual(1, jokes.Count);
            Assert.IsTrue(jokes.Single() == JokeTestWithCategory);
        }

        [Test]
        public void GetRandomJokesWorksWithReplacementName()
        {
            const string nameToReplace = "Joke";

            var randomNameFeed = GetRandomNameMock();
            var helper = GetJokeHelperWithMockFeed(randomNameFeed.Object);
            var jokes = helper.GetRandomJokes(1, nameToReplace:nameToReplace).ToList();

            Assert.AreEqual(1, jokes.Count);
            Assert.IsTrue(jokes.Single() == JokeTest.Replace(nameToReplace, ReplacementName));
            
            randomNameFeed.Verify(v => v.GetRandomFullName(), Times.Once);
            randomNameFeed.VerifyNoOtherCalls();
        }

        [Test]
        public void GetRandomJokesWorksWithReplacementNameAndCategory()
        {
            const string nameToReplace = "Joke";

            var randomNameFeed = GetRandomNameMock();
            var helper = GetJokeHelperWithMockFeed(randomNameFeed.Object);
            var jokes = helper.GetRandomJokes(1, "category2",  nameToReplace).ToList();

            Assert.AreEqual(1, jokes.Count);
            Assert.IsTrue(jokes.Single() == JokeTestWithCategory.Replace(nameToReplace, ReplacementName));
        }

        private static JokeHelper GetJokeHelperWithMockFeed(IGetRandomName randomNameFeed)
        {
            var jokeFeed = new Mock<IGetRandomJokes>();
            jokeFeed.Setup(j => j.GetRandomJoke(It.IsAny<string>())).Returns(JokeTestWithCategory);
            jokeFeed.Setup(j => j.GetRandomJoke(null)).Returns(JokeTest);

            return new JokeHelper(jokeFeed.Object, randomNameFeed);
        }

        private static Mock<IGetRandomName> GetRandomNameMock()
        {
            var nameFeed = new Mock<IGetRandomName>();
            nameFeed.Setup(j => j.GetRandomFullName()).Returns(ReplacementName);

            return nameFeed;
        }
    }
}
