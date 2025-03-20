using MonsterTradingCardsGame.API.Server;
using HttpMethod = MonsterTradingCardsGame.API.Server.HttpMethod;

namespace MonsterTradingCardsGame.Tests
{
    internal class HttpMethodTests
    {
        // arrange
        private static readonly string[] ValidPostStrings = ["post", "POST", "Post"];

        [Test, TestCaseSource(nameof(ValidPostStrings))]
        public void PostStringReturnsPostMethod(string validPostString)
        {
            // act
            var actualMethod = HttpMethodUtilities.GetMethod(validPostString);

            // assert
            Assert.That(actualMethod, Is.EqualTo(HttpMethod.Post));
        }

        [Test]
        public void UnknownStringThrowsInvalidDataException()
        {
            // arrange
            const string invalidString = "INVALID";

            // act and assert
            Assert.That(() => HttpMethodUtilities.GetMethod(invalidString), Throws.TypeOf<InvalidDataException>());
        }
    }
}
