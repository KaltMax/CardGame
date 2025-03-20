using MonsterTradingCardsGame.BLL.Models;
using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.Tests
{
    internal class DeckTests
    {
        [Test]
        public void Constructor_ShouldInitializePlayerDeck_ForValidUserDeckDTO()
        {
            // Arrange
            var userDeckDto = new UserDeckDTO
            {
                Cards = new List<CardDTO>
                {
                    new("1", "Fire Dragon", 50),
                    new("2", "Water Spell", 30)
                }
            };

            // Act
            var deck = new Deck(userDeckDto);

            // Assert
            Assert.That(deck.PlayerDeck, Is.Not.Null);
            Assert.That(deck.PlayerDeck.Count, Is.EqualTo(2));
            Assert.That(deck.PlayerDeck[0].Name, Is.EqualTo("Fire Dragon"));
            Assert.That(deck.PlayerDeck[1].Name, Is.EqualTo("Water Spell"));
        }

        [Test]
        public void Constructor_ShouldThrowException_WhenUserDeckDTOIsNull()
        {
            // Arrange
            UserDeckDTO? playerDeck = null;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Deck(playerDeck!));
        }

        [Test]
        public void Constructor_ShouldThrowException_WhenUserDeckDTOIsEmpty()
        {
            // Arrange
            var userDeck = new UserDeckDTO
            {
                Cards = new List<CardDTO>()
            };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Deck(userDeck));
        }

        [Test]
        public void AddCard_ShouldAddCardToPlayerDeck()
        {
            // Arrange
            var deck = new Deck();
            var card = new Card(new CardDTO("1", "Fire Dragon", 50));

            // Act
            deck.AddCard(card);

            // Assert
            Assert.That(deck.PlayerDeck, Contains.Item(card));
        }

        [Test]
        public void RemoveCard_ShouldRemoveCardFromPlayerDeck()
        {
            // Arrange
            var card = new Card(new CardDTO("1", "Fire Dragon", 50));
            var deck = new Deck
            {
                PlayerDeck = new List<Card> { card }
            };

            // Act
            deck.RemoveCard(card);

            // Assert
            Assert.That(deck.PlayerDeck, Does.Not.Contain(card));
        }
    }
}
