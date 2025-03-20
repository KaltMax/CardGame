using MonsterTradingCardsGame.BLL.Enums;
using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Models
{
    public class Card
    {
        public string Name { get; init; }
        public float Damage { get; init; }
        public ElementType Element { get; init; }
        public CardType Type { get; init; }

        public Card(CardDTO cardDto)
        {
            Name = cardDto.Name;
            Damage = cardDto.Damage;
            Element = DetermineCardElement(cardDto.Name);
            Type = DetermineCardType(cardDto.Name);
        }

        private ElementType DetermineCardElement(string cardName)
        {
            if (cardName.StartsWith("Water", StringComparison.OrdinalIgnoreCase))
                return ElementType.Water;
            if (cardName.StartsWith("Fire", StringComparison.OrdinalIgnoreCase))
                return ElementType.Fire;
            if (cardName.StartsWith("Regular", StringComparison.OrdinalIgnoreCase))
                return ElementType.Normal;

            return ElementType.Normal;
        }

        private CardType DetermineCardType(string cardName)
        {
            if (cardName.Contains("Spell", StringComparison.OrdinalIgnoreCase))
                return CardType.Spell;
            if (cardName.Contains("Dragon", StringComparison.OrdinalIgnoreCase))
                return CardType.Dragon;
            if (cardName.Contains("Goblin", StringComparison.OrdinalIgnoreCase))
                return CardType.Goblin;
            if (cardName.Contains("Elf", StringComparison.OrdinalIgnoreCase))
                return CardType.Elf;
            if (cardName.Contains("Ork", StringComparison.OrdinalIgnoreCase))
                return CardType.Ork;
            if (cardName.Contains("Knight", StringComparison.OrdinalIgnoreCase))
                return CardType.Knight;
            if (cardName.Contains("Kraken", StringComparison.OrdinalIgnoreCase))
                return CardType.Kraken;
            if (cardName.Contains("Wizzard", StringComparison.OrdinalIgnoreCase))
                return CardType.Wizzard;

            return CardType.Unknown;
        }
    }
}
