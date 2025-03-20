using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.API.Commands;
using MonsterTradingCardsGame.BLL.Services;
using MonsterTradingCardsGame.BLL.Exceptions;
using NSubstitute;
using MonsterTradingCardsGame.DTOs;
using NSubstitute.ExceptionExtensions;

namespace MonsterTradingCardsGame.Tests
{
    internal class AcquirePackageCommandTests
    {
        private IPackageService _mockPackageService;
        private ITokenService _mockTokenService;
        private AcquirePackageCommand _command;

        private const string ValidToken = "valid-token";
        private const string InvalidToken = "invalid-token";
        private const string Username = "testuser";

        [SetUp]
        public void Setup()
        {
            _mockPackageService = Substitute.For<IPackageService>();
            _mockTokenService = Substitute.For<ITokenService>();
            _command = new AcquirePackageCommand(_mockPackageService, _mockTokenService);
        }

        private HttpRequest CreateRequest(string token)
        {
            return new HttpRequest
            {
                Header = new Dictionary<string, string> { { "Authorization", token } }
            };
        }

        [Test]
        public void Execute_ValidRequest_ShouldReturnCreatedResponse()
        {
            // Arrange
            var request = CreateRequest(ValidToken);
            var package = new PackageDTO([
                new CardDTO("Card1Id", "Card1Name", 25),
                new CardDTO("Card2Id", "Card2Name", 25),
                new CardDTO("Card3Id", "Card3Name", 25),
                new CardDTO("Card4Id", "Card4Name", 25),
                new CardDTO("Card5Id", "Card5Name", 25)
            ]);

            _mockTokenService.GetUsernameFromToken(ValidToken).Returns(Username);
            _mockTokenService.ValidateToken(ValidToken, Username);
            _mockPackageService.AcquirePackage(Username).Returns(package);

            // Act
            var response = _command.Execute(request);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(StatusCode.Created));
                Assert.That(response.ContentType, Is.EqualTo("application/json; charset=utf-8"));
                Assert.That(response.Payload, Does.Contain("Card1Id"));
            });
        }

        [Test]
        public void Execute_InvalidToken_ShouldReturnUnauthorizedResponse()
        {
            // Arrange
            var request = CreateRequest(InvalidToken);
            _mockTokenService.GetUsernameFromToken(InvalidToken).Throws<UnauthorizedAccessException>();

            // Act
            var response = _command.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(StatusCode.Unauthorized));
            Assert.That(response.Payload, Does.Contain("401 Unauthorized"));

        }

        [Test]
        public void Execute_InsufficientFunds_ShouldReturnForbiddenResponse()
        {
            // Arrange
            var request = CreateRequest(ValidToken);
            _mockTokenService.GetUsernameFromToken(ValidToken).Returns(Username);
            _mockTokenService.ValidateToken(ValidToken, Username);
            _mockPackageService.AcquirePackage(Username).Throws<InsufficientFundsException>();

            // Act
            var response = _command.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(StatusCode.Forbidden));
            Assert.That(response.Payload, Does.Contain("403 Forbidden"));
        }

        [Test]
        public void Execute_NoPackagesAvailable_ShouldReturnNotFoundResponse()
        {
            // Arrange
            var request = CreateRequest(ValidToken);
            _mockTokenService.GetUsernameFromToken(ValidToken).Returns(Username);
            _mockTokenService.ValidateToken(ValidToken, Username);
            _mockPackageService.AcquirePackage(Username).Throws<NoPackagesAvailableException>();

            // Act
            var response = _command.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(StatusCode.NotFound));
            Assert.That(response.Payload, Does.Contain("404 Not Found"));
        }

        [Test]
        public void Execute_UnhandledException_ShouldReturnInternalServerErrorResponse()
        {
            // Arrange
            var request = CreateRequest(ValidToken);
            _mockTokenService.GetUsernameFromToken(ValidToken).Returns(Username);
            _mockTokenService.ValidateToken(ValidToken, Username);
            _mockPackageService.AcquirePackage(Username).Throws<Exception>();

            // Act
            var response = _command.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(StatusCode.InternalServerError));
            Assert.That(response.Payload, Does.Contain("500 Internal Server Error"));
        }

        [Test]
        public void Execute_UserDoesNotExist_ShouldReturnNotFoundResponse()
        {
            // Arrange
            var request = CreateRequest(ValidToken);
            _mockTokenService.GetUsernameFromToken(ValidToken).Returns(Username);
            _mockTokenService.ValidateToken(ValidToken, Username);
            _mockPackageService.AcquirePackage(Username).Throws<UserDoesNotExistException>();

            // Act
            var response = _command.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(StatusCode.NotFound));
            Assert.That(response.Payload, Does.Contain("404 Not Found"));
        }

        [Test]
        public void Execute_AcquirePackageException_ShouldReturnInternalServerErrorResponse()
        {
            // Arrange
            var request = CreateRequest(ValidToken);
            _mockTokenService.GetUsernameFromToken(ValidToken).Returns(Username);
            _mockTokenService.ValidateToken(ValidToken, Username);
            _mockPackageService.AcquirePackage(Username).Throws<AcquirePackageException>();

            // Act
            var response = _command.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(StatusCode.InternalServerError));
            Assert.That(response.Payload, Does.Contain("500 Internal Server Error"));
        }
    }
}
