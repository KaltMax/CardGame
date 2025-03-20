using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Models;

namespace MonsterTradingCardsGame.Tests
{
    [NonParallelizable]
    internal class LobbyTests
    {
        private Lobby _lobby;

        [SetUp]
        public void Setup()
        {
            _lobby = new Lobby();
        }

        [Test]
        public void EnterLobby_ShouldStartBattle_WhenAnotherPlayerIsWaiting()
        {
            // Arrange
            var player1 = new User("Player1", new Deck(), 0, 0, 0);
            var player2 = new User("Player2", new Deck(), 0, 0, 0);

            // Simulate player1 entering lobby
            var task1 = Task.Run(() => _lobby.EnterLobby(player1));

            // Simulate player2 entering lobby
            var task2 = Task.Run(() => _lobby.EnterLobby(player2));

            // Wait for tasks to complete
            Task.WaitAll(task1, task2);

            // Assert
            Assert.That(task1.Result, Is.Not.Null, "Battle log should be returned for Player1.");
            Assert.That(task2.Result, Is.Not.Null, "Battle log should be returned for Player2.");
            Assert.That(task1.Result, Is.EqualTo(task2.Result), "Both players should receive the same battle log.");
        }

        [Test]
        public void EnterLobby_ShouldHandleMultiplePlayersConcurrently()
        {
            // Arrange
            var players = new List<User>();
            for (var i = 1; i <= 20; i++)
            {
                players.Add(new User($"Player{i}", new Deck(), 0, 0, 0));
            }

            var tasks = players.Select(player => Task.Run(() => _lobby.EnterLobby(player))).ToArray();

            // Act
            Task.WaitAll(tasks);

            // Assert
            foreach (var task in tasks)
            {
                Assert.That(task.Result, Is.Not.Null, "Each player should receive a battle log.");
            }

            var uniqueBattleLogs = tasks.Select(t => t.Result).Distinct().Count();
            Assert.That(uniqueBattleLogs, Is.EqualTo(10), "10 unique battle logs should exist (10 battles).");
        }

        [Test]
        public void EnterLobby_ShouldThrowBattleFailedException_WhenPlayerAlreadyInLobby()
        {
            // Arrange
            var player = new User("Player1", new Deck(), 0, 0, 0);

            // Task 1: Enter the lobby
            var task1 = Task.Run(() => _lobby.EnterLobby(player));

            // Task 2: Attempt to enter the lobby again with the same player
            var task2 = Task.Run(() =>
            {
                try
                {
                    _lobby.EnterLobby(player);
                }
                catch (PlayerAlreadyInLobbyOrBattleException ex)
                {
                    Assert.That(ex.Message, Is.EqualTo("Player is already in the lobby or in a battle."));
                }
            });

            // Cleanup
            Task.Run(() => _lobby.EnterLobby(new User("Player2", new Deck(), 0, 0, 0)));

            // Wait for all tasks to complete
            Task.WhenAll(task1, task2);
        }
    }
}
