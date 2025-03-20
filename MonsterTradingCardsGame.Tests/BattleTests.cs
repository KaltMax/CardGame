using MonsterTradingCardsGame.BLL.Models;
using MonsterTradingCardsGame.DTOs;
using NSubstitute;

namespace MonsterTradingCardsGame.Tests
{
    internal class BattleTests
    {
        [Test]
        public void DetermineWinner_ShouldReturnPlayer1_WhenPlayer2DeckIsEmpty()
        {
            // Arrange
            var player1Deck = new Deck { PlayerDeck = [CreateMockCard("Dragon", 50)] };
            var player2Deck = Substitute.For<Deck>();

            var player1 = Substitute.For<User>("Player1", player1Deck, 100, 0, 0);
            var player2 = Substitute.For<User>("Player2", player2Deck, 100, 0, 0);

            var battle = new Battle(player1, player2);

            // Act
            var winner = battle.DetermineWinner();

            // Assert
            Assert.That(winner, Is.EqualTo(player1));
        }

        [Test]
        public void DetermineWinner_ShouldReturnPlayer2_WhenPlayer1DeckIsEmpty()
        {
            // Arrange
            var player1Deck = Substitute.For<Deck>();
            var player2Deck = new Deck { PlayerDeck = [CreateMockCard("Dragon", 50)] };

            var player1 = Substitute.For<User>("Player1", player1Deck, 100, 0, 0);
            var player2 = Substitute.For<User>("Player2", player2Deck, 100, 0, 0);

            var battle = new Battle(player1, player2);

            // Act
            var winner = battle.DetermineWinner();

            // Assert
            Assert.That(winner, Is.EqualTo(player2));
        }

        [Test]
        public void StartBattle_ShouldResultInDraw_WhenBothPlayersHaveCardsAfter100Rounds()
        {
            // Arrange
            var player1Deck = new Deck { PlayerDeck = [CreateMockCard("Dragon", 50)] };
            var player2Deck = new Deck { PlayerDeck = [CreateMockCard("Dragon", 50)] };

            var player1 = Substitute.For<User>("Player1", player1Deck, 100, 0, 0);
            var player2 = Substitute.For<User>("Player2", player2Deck, 100, 0, 0);

            var battle = new Battle(player1, player2);

            // Act
            battle.StartBattle();

            // Assert
            Assert.That(battle.BattleLog["Result"], Is.EqualTo("The battle ended in a draw. Elos stay unchanged."));
            Assert.That(player1.Elo, Is.EqualTo(100));
            Assert.That(player2.Elo, Is.EqualTo(100));
        }

        [Test]
        [TestCase("Player1", "Player2", 50, 30, true)] // Player1 wins
        [TestCase("Player1", "Player2", 30, 50, false)] // Player2 wins
        public void UpdatePlayerDecksAfterRound_ShouldUpdatePlayerDecks(string player1Name, string player2Name, float player1Damage, float player2Damage, bool isPlayer1Winner)
        {
            // Arrange
            var player1Deck = new Deck { PlayerDeck = [CreateMockCard("Card1", player1Damage)] };
            var player2Deck = new Deck { PlayerDeck = [CreateMockCard("Card2", player2Damage)] };

            var player1 = Substitute.For<User>(player1Name, player1Deck, 100, 0, 0);
            var player2 = Substitute.For<User>(player2Name, player2Deck, 100, 0, 0);

            var battle = new Battle(player1, player2);
            var player1Card = player1Deck.PlayerDeck[0];
            var player2Card = player2Deck.PlayerDeck[0];

            // Act
            var winner = isPlayer1Winner ? player1 : player2;
            battle.UpdatePlayerDecksAfterRound(winner, player1Card, player2Card);

            // Assert
            if (isPlayer1Winner)
            {
                Assert.That(player1.Deck.PlayerDeck, Contains.Item(player2Card));
                Assert.That(player2.Deck.PlayerDeck, Does.Not.Contain(player2Card));
            }
            else
            {
                Assert.That(player2.Deck.PlayerDeck, Contains.Item(player1Card));
                Assert.That(player1.Deck.PlayerDeck, Does.Not.Contain(player1Card));
            }
        }

        [Test]
        public void AddBattleLog_ShouldAddLogEntry()
        {
            // Arrange
            var player1 = Substitute.For<User>("Player1", Substitute.For<Deck>(), 100, 0, 0);
            var player2 = Substitute.For<User>("Player2", Substitute.For<Deck>(), 100, 0, 0);
            var battle = new Battle(player1, player2);

            // Act
            battle.AddBattleLog("Round 1", "Player1 attacked Player2.");

            // Assert
            Assert.That(battle.BattleLog.ContainsKey("Round 1"));
            Assert.That(battle.BattleLog["Round 1"], Is.EqualTo("Player1 attacked Player2."));
        }

        [Test]
        public void UpdatePlayerStatsAfterBattle_ShouldUpdateWinnerAndLoserStats()
        {
            // Arrange
            var player1Deck = new Deck { PlayerDeck = [CreateMockCard("Dragon", 50)] };

            var player2Deck = Substitute.For<Deck>();

            var player1 = Substitute.For<User>("Player1", player1Deck, 100, 0, 0);
            var player2 = Substitute.For<User>("Player2", player2Deck, 100, 0, 0);

            var battle = new Battle(player1, player2);

            // Act
            battle.UpdatePlayerStatsAfterBattle(player1);

            // Assert
            Assert.That(player1.Wins, Is.EqualTo(1));
            Assert.That(player1.Losses, Is.EqualTo(0));
            Assert.That(player1.Elo, Is.EqualTo(103));
            Assert.That(player2.Wins, Is.EqualTo(0));
            Assert.That(player2.Losses, Is.EqualTo(1));
            Assert.That(player2.Elo, Is.EqualTo(95));
        }

        [Test]
        public void UserUpdateStats_EloShouldNotGetBelowZero()
        {
            var player1Deck = new Deck { PlayerDeck = [CreateMockCard("Dragon", 50)] };
            var player1 = Substitute.For<User>("Player1", player1Deck, 0, 0, 0);

            player1.UpdateStats(false);

            Assert.That(player1.Elo, Is.EqualTo(0));
        }

        private static Card CreateMockCard(string name, float damage)
        {
            // Use real Card instance instead of mocking
            return new Card(new CardDTO(Guid.NewGuid().ToString(), name, damage));
        }
    }
}
