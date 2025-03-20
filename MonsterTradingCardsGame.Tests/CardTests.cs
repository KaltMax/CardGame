using MonsterTradingCardsGame.BLL.Models;
using MonsterTradingCardsGame.BLL.Enums;
using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.Tests
{
    internal class CardTests
    {
        [Test]
        public void Constructor_ShouldInitializePropertiesCorrectly_ForValidCardDTO()
        {
            // Arrange
            var cardDto = new CardDTO("1", "WaterSpell", 30);

            // Act
            var card = new Card(cardDto);

            // Assert
            Assert.That(card.Name, Is.EqualTo("WaterSpell"));
            Assert.That(card.Damage, Is.EqualTo(30));
            Assert.That(card.Type, Is.EqualTo(CardType.Spell));
            Assert.That(card.Element, Is.EqualTo(ElementType.Water));
        }

        [Test]
        public void DetermineCardType_ShouldReturnMonster_WhenNameDoesNotContainSpell()
        {
            // Arrange
            var cardDto = new CardDTO("1", "Goblin", 50);

            // Act
            var card = new Card(cardDto);

            // Assert
            Assert.That(card.Type, Is.EqualTo(CardType.Goblin));
        }

        [Test]
        public void DetermineCardElement_ShouldReturnFire_WhenNameStartsWithFire()
        {
            // Arrange
            var cardDto = new CardDTO("1", "FireSpell", 40);

            // Act
            var card = new Card(cardDto);

            // Assert
            Assert.That(card.Element, Is.EqualTo(ElementType.Fire));
        }

        [Test]
        public void DetermineCardElement_ShouldReturnNormal_WhenNoElementIsSpecified()
        {
            // Arrange
            var cardDto = new CardDTO("1", "Wizzard", 25);

            // Act
            var card = new Card(cardDto);

            // Assert
            Assert.That(card.Element, Is.EqualTo(ElementType.Normal));
        }
    }
}
