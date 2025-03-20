using MonsterTradingCardsGame.BLL.Enums;
using MonsterTradingCardsGame.BLL.Models;
using MonsterTradingCardsGame.DTOs;
using NSubstitute;

namespace MonsterTradingCardsGame.Tests
{
    internal class RulesTests
    {
        private Rules _rules;

        [SetUp]
        public void SetUp()
        {
            _rules = new Rules();
        }

        [Test]
        public void DetermineRoundWinner_ShouldApplyGoblinDragonRule()
        {
            // Arrange
            var goblinCard = Substitute.For<Card>(new CardDTO("1", "Goblin", 10));
            var dragonCard = Substitute.For<Card>(new CardDTO("2", "Fire Dragon", 50));

            var player1 = Substitute.For<User>("Player1", null, 0, 0, 0);
            var player2 = Substitute.For<User>("Player2", null, 0, 0, 0);

            // Act
            var (winner, log) = _rules.DetermineRoundWinner(goblinCard, dragonCard, player1, player2);

            // Assert
            Assert.That(winner, Is.EqualTo(player2));
            Assert.That(log, Does.Contain("scares"));
        }

        [Test]
        public void DetermineRoundWinner_ShouldApplyWizzardOrkRule()
        {
            // Arrange
            var wizzardCard = Substitute.For<Card>(new CardDTO("1", "Wizzard", 20));
            var orkCard = Substitute.For<Card>(new CardDTO("2", "Ork", 40));

            var player1 = Substitute.For<User>("Player1", null, 0, 0, 0);
            var player2 = Substitute.For<User>("Player2", null, 0, 0, 0);

            // Act
            var (winner, log) = _rules.DetermineRoundWinner(wizzardCard, orkCard, player1, player2);

            // Assert
            Assert.That(winner, Is.EqualTo(player1));
            Assert.That(log, Does.Contain("controls"));
        }

        [Test]
        public void DetermineRoundWinner_ShouldApplyKrakenImmunityToSpells()
        {
            // Arrange
            var krakenCard = Substitute.For<Card>(new CardDTO("1", "Kraken", 30));
            var spellCard = Substitute.For<Card>(new CardDTO("2", "Water Spell", 50));

            var player1 = Substitute.For<User>("Player1", null, 0, 0, 0);
            var player2 = Substitute.For<User>("Player2", null, 0, 0, 0);

            // Act
            var (winner, log) = _rules.DetermineRoundWinner(krakenCard, spellCard, player1, player2);

            // Assert
            Assert.That(winner, Is.EqualTo(player1));
            Assert.That(log, Does.Contain("is immune to"));
        }

        [Test]
        public void DetermineRoundWinner_ShouldApplyWaterSpellKnightRule()
        {
            // Arrange
            var waterSpellCard = Substitute.For<Card>(new CardDTO("1", "Water Spell", 30));
            var knightCard = Substitute.For<Card>(new CardDTO("2", "Knight", 50));

            var player1 = Substitute.For<User>("Player1", null, 0, 0, 0);
            var player2 = Substitute.For<User>("Player2", null, 0, 0, 0);

            // Act
            var (winner, log) = _rules.DetermineRoundWinner(knightCard, waterSpellCard, player1, player2);

            // Assert
            Assert.That(winner, Is.EqualTo(player2));
            Assert.That(log, Does.Contain("drowns"));
        }

        [Test]
        public void DetermineRoundWinner_ShouldApplyFireElfDragonRule()
        {
            // Arrange
            var fireElfCard = Substitute.For<Card>(new CardDTO("1", "FireElf", 30));
            var dragonCard = Substitute.For<Card>(new CardDTO("2", "Dragon", 50));

            var player1 = Substitute.For<User>("Player1", null, 0, 0, 0);
            var player2 = Substitute.For<User>("Player2", null, 0, 0, 0);

            // Act
            var (winner, log) = _rules.DetermineRoundWinner(fireElfCard, dragonCard, player1, player2);

            // Assert
            Assert.That(winner, Is.EqualTo(player1));
            Assert.That(log, Does.Contain("evades"));
        }

        [Test]
        public void DetermineRoundWinner_ShouldApplyElementalDamageMultiplier()
        {
            // Arrange
            var waterCard = Substitute.For<Card>(new CardDTO("1", "WaterDragon", 30));
            var fireCard = Substitute.For<Card>(new CardDTO("2", "FireSpell", 20));

            var player1 = Substitute.For<User>("Player1", null, 0, 0, 0);
            var player2 = Substitute.For<User>("Player2", null, 0, 0, 0);

            // Act
            var (winner, log) = _rules.DetermineRoundWinner(waterCard, fireCard, player1, player2);

            // Assert
            Assert.That(winner, Is.EqualTo(player1));
            Assert.That(log, Does.Contain("overpowers"));
        }

        [Test]
        public void DetermineRoundWinner_ShouldReturnDraw_WhenDamageIsEqual()
        {
            // Arrange
            var card1 = Substitute.For<Card>(new CardDTO("1", "Regular Card", 30));
            var card2 = Substitute.For<Card>(new CardDTO("2", "Regular Card", 30));

            var player1 = Substitute.For<User>("Player1", null, 0, 0, 0);
            var player2 = Substitute.For<User>("Player2", null, 0, 0, 0);

            // Act
            var (winner, log) = _rules.DetermineRoundWinner(card1, card2, player1, player2);

            // Assert
            Assert.That(winner, Is.Null);
            Assert.That(log, Does.Contain("draw"));
        }

        [TestCase(ElementType.Water, ElementType.Fire, 30f, ExpectedResult = 60f)]  // Water is effective against Fire
        [TestCase(ElementType.Fire, ElementType.Water, 30f, ExpectedResult = 15f)] // Fire is weak against Water
        [TestCase(ElementType.Fire, ElementType.Normal, 30f, ExpectedResult = 60f)] // Fire is effective against Normal
        [TestCase(ElementType.Normal, ElementType.Fire, 30f, ExpectedResult = 15f)] // Normal is weak against Fire
        [TestCase(ElementType.Normal, ElementType.Water, 30f, ExpectedResult = 60f)] // Normal is effective against Water
        [TestCase(ElementType.Water, ElementType.Normal, 30f, ExpectedResult = 15f)] // Water is weak against Normal
        [TestCase(ElementType.Water, ElementType.Water, 30f, ExpectedResult = 30f)] // Same element
        [TestCase(ElementType.Fire, ElementType.Fire, 30f, ExpectedResult = 30f)]   // Same element
        [TestCase(ElementType.Normal, ElementType.Normal, 30f, ExpectedResult = 30f)] // Same element

        public float AdjustDamageForElement_ShouldReturnCorrectAdjustedDamage(ElementType element1, ElementType element2, float damage)
        {
            // Arrange
            var card1 = new Card(new CardDTO("1", "Card1", damage)) { Element = element1 };
            var card2 = new Card(new CardDTO("2", "Card2", damage)) { Element = element2 };

            // Act
            return _rules.AdjustDamageForElement(card1, card2);
        }
    }
}
