using MonsterTradingCardsGame.API.Routing;
using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.API.Commands;
using MonsterTradingCardsGame.BLL.Services;
using HttpMethod = MonsterTradingCardsGame.API.Server.HttpMethod;
using NSubstitute;

namespace MonsterTradingCardsGame.Tests
{
    internal class RouteRequestTests
    {
        [Test]
        public void RouteValidRequestShouldExecuteCorrectCommand()
        {
            // Arrange
            var mockCommand = Substitute.For<IRequestCommand>();
            mockCommand.Execute(Arg.Any<HttpRequest>()).Returns(new HttpResponse
            {
                StatusCode = StatusCode.Ok,
                Payload = "Command Executed"
            });

            var router = CreateMockRouter();

            // Inject a custom route
            InjectRoutes(router, new List<(string, HttpMethod, IRequestCommand)>
            {
                ("/test", HttpMethod.Post, mockCommand)
            });

            var request = new HttpRequest
            {
                Method = HttpMethod.Post,
                ResourcePath = "/test"
            };

            // Act
            var response = router.Route(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(StatusCode.Ok));
            Assert.That(response.Payload, Is.EqualTo("Command Executed"));
            mockCommand.Received(1).Execute(Arg.Any<HttpRequest>());
        }

        [Test]
        public void RouteInvalidRequestShouldReturnNotFound()
        {
            // Arrange
            var router = CreateMockRouter();

            var request = new HttpRequest
            {
                Method = HttpMethod.Get,
                ResourcePath = "/invalid"
            };

            // Act
            var response = router.Route(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(StatusCode.NotFound));
            Assert.That(response.Payload, Is.EqualTo("404 Not Found"));
        }

        [Test]
        public void RouteRequestWithDynamicSegmentShouldExecuteCorrectCommand()
        {
            // Arrange
            var mockCommand = Substitute.For<IRequestCommand>();
            mockCommand.Execute(Arg.Any<HttpRequest>()).Returns(new HttpResponse
            {
                StatusCode = StatusCode.Ok,
                Payload = "Dynamic Command Executed"
            });

            var router = CreateMockRouter();

            // Inject a custom route that allows one dynamic segment
            InjectRoutes(router, [("/users/{username}", HttpMethod.Get, mockCommand)]);

            // Make a request with one extra segment: "/users/mustermax"
            var request = new HttpRequest
            {
                Method = HttpMethod.Get,
                ResourcePath = "/users/mustermax"
            };

            // Act
            var response = router.Route(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(StatusCode.Ok));
            Assert.That(response.Payload, Is.EqualTo("Dynamic Command Executed"));
            mockCommand.Received(1).Execute(Arg.Any<HttpRequest>());
        }

        [Test]
        public void RouteRequestWithExtraDynamicSegmentShouldReturnNotFound()
        {
            // Arrange
            var router = CreateMockRouter();

            // No matching route for "/users/mustermax/details" 
            var request = new HttpRequest
            {
                Method = HttpMethod.Get,
                ResourcePath = "/users/mustermax/details"
            };

            // Act
            var response = router.Route(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(StatusCode.NotFound));
            Assert.That(response.Payload, Is.EqualTo("404 Not Found"));
        }

        private Router CreateMockRouter()
        {
            var mockUserService = Substitute.For<IUserService>();
            var mockTokenService = Substitute.For<ITokenService>();
            var mockGameService = Substitute.For<IGameService>();
            var mockCardService = Substitute.For<ICardService>();
            var mockPackageService = Substitute.For<IPackageService>();
            var mockTradingsService = Substitute.For<ITradingsService>();
            var mockFriendsService = Substitute.For<IFriendsService>();

            return new Router(
                mockUserService,
                mockTokenService,
                mockGameService,
                mockCardService,
                mockPackageService,
                mockTradingsService,
                mockFriendsService
            );
        }

        private void InjectRoutes(Router router, List<(string BasePath, HttpMethod Method, IRequestCommand Command)> routes)
        {
            var field = typeof(Router).GetField("_routes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field?.SetValue(router, routes);
        }
    }
}
