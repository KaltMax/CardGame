using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;
using MonsterTradingCardsGame.DAL.Exceptions;
using MonsterTradingCardsGame.DAL.Repositories;
using MonsterTradingCardsGame.DTOs;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace MonsterTradingCardsGame.Tests
{
    internal class GameServiceTests
    {
        private IGameRepository _mockGameRepository;
        private ICardRepository _mockCardRepository;
        private GameService _gameService;

        [SetUp]
        public void SetUp()
        {
            _mockGameRepository = Substitute.For<IGameRepository>();
            _mockCardRepository = Substitute.For<ICardRepository>();
            _gameService = new GameService(_mockGameRepository, _mockCardRepository);
        }

        [Test]
        public void GetUserStats_ShouldReturnUserStats_ForValidUsername()
        {
            // Arrange
            var username = "testuser";
            var expectedStats = new UserStatsDTO(username, 1200, 10, 5);
            _mockGameRepository.GetUserStats(username).Returns(expectedStats);

            // Act
            var result = _gameService.GetUserStats(username);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Elo, Is.EqualTo(1200));
            Assert.That(result.Wins, Is.EqualTo(10));
            Assert.That(result.Losses, Is.EqualTo(5));
        }

        [Test]
        public void GetUserStats_ShouldThrowUserDoesNotExistException_ForInvalidUsername()
        {
            // Arrange
            var username = "invaliduser";
            _mockGameRepository.GetUserStats(username).Throws(new UserNotFoundException());

            // Act & Assert
            Assert.Throws<UserDoesNotExistException>(() => _gameService.GetUserStats(username));
        }

        [Test]
        public void GetUserStats_ShouldThrowDatabaseException_ForErrorWhileRetrievingStatsFromDatabase()
        {
            // Arrange
            var username = "invaliduser";
            _mockGameRepository.GetUserStats(username).Throws(new Exception());

            // Act & Assert
            Assert.Throws<StatsRetrievalException>(() => _gameService.GetUserStats(username));
        }

        [Test]
        public void GetScoreboard_ShouldReturnScoreboard()
        {
            // Arrange
            var expectedScoreboard = new List<UserStatsDTO>
            {
                new("user1", 100, 15, 5),
                new("user2", 105, 10, 10)
            };
            _mockGameRepository.GetScoreboard().Returns(expectedScoreboard);

            // Act
            var result = _gameService.GetScoreboard();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("user1"));
            Assert.That(result[0].Elo, Is.EqualTo(100));
            Assert.That(result[0].Wins, Is.EqualTo(15));
            Assert.That(result[0].Losses, Is.EqualTo(5));

            Assert.That(result[1].Name, Is.EqualTo("user2"));
            Assert.That(result[1].Elo, Is.EqualTo(105));
            Assert.That(result[1].Wins, Is.EqualTo(10));
            Assert.That(result[1].Losses, Is.EqualTo(10));
        }

        [Test]
        public void GetScoreboard_ShouldThrowScoreboardRetrievalException_OnFailure()
        {
            // Arrange
            _mockGameRepository.GetScoreboard().Throws(new Exception());

            // Act & Assert
            Assert.Throws<ScoreboardRetrievalException>(() => _gameService.GetScoreboard());
        }

        [Test]
        public void EnterLobby_ShouldThrowUserDoesNotExistException_ForInvalidUser()
        {
            // Arrange
            var username = "invaliduser";
            _mockGameRepository.GetUserStats(username).Throws(new UserNotFoundException());

            // Act & Assert
            Assert.Throws<UserDoesNotExistException>(() => _gameService.EnterLobby(username));
        }

        [Test]
        public void ConvertBattleLogToString_ShouldReturnFormattedString()
        {
            // Arrange
            var battleLog = new Dictionary<string, string>
            {
                { "Battle", "Player1 vs Player2" },
                { "Round 1", "Player1 attacks Player2" },
                { "Result", "Player1 wins" }
            };

            // Act
            var result = _gameService.ConvertBattleLogToString(battleLog);

            // Assert
            Assert.That(result, Does.Contain("Battle:"));
            Assert.That(result, Does.Contain("Player1 vs Player2"));
            Assert.That(result, Does.Contain("Round 1:"));
            Assert.That(result, Does.Contain("Player1 wins"));
        }
    }
}
