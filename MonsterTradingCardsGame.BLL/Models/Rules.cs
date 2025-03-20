using MonsterTradingCardsGame.BLL.Enums;

namespace MonsterTradingCardsGame.BLL.Models
{
    public class Rules
    {
        public (User? winner, string logEntry) DetermineRoundWinner(Card card1, Card card2, User player1, User player2)
        {
            // Check special rules

            // Goblin is scared of Dragon
            if (card1.Type is CardType.Goblin && card2.Type is CardType.Dragon)
                return (player2, $"{player2.Name}'s {card2.Name} (Damage: {card2.Damage}) scares {player1.Name}'s {card1.Name} (Damage: {card1.Damage}). {player2.Name} wins the round.");
            if (card2.Type is CardType.Goblin && card1.Type is CardType.Dragon)
                return (player1, $"{player1.Name}'s {card1.Name} (Damage: {card1.Damage}) scares {player2.Name}'s {card2.Name} (Damage: {card2.Damage}). {player1.Name} wins the round.");

            // Wizzard controls Ork
            if (card1.Type is CardType.Wizzard && card2.Type is CardType.Ork)
                return (player1, $"{player1.Name}'s {card1.Name} (Damage: {card1.Damage}) controls {player2.Name}'s {card2.Name} (Damage: {card2.Damage}). {player1.Name} wins the round.");
            if (card2.Type is CardType.Wizzard && card1.Type is CardType.Ork)
                return (player2, $"{player2.Name}'s {card2.Name} (Damage: {card2.Damage}) controls {player1.Name}'s {card1.Name} (Damage: {card1.Damage}). {player2.Name} wins the round.");

            // WaterSpell drowns Knight
            if (card1.Type is CardType.Knight && (card2.Type is CardType.Spell && card2.Element is ElementType.Water))
                return (player2, $"{player2.Name}'s {card2.Name} (Damage: {card2.Damage}) drowns {player1.Name}'s {card1.Name} (Damage: {card1.Damage}). {player2.Name} wins the round.");
            if (card2.Type is CardType.Knight && (card1.Type is CardType.Spell && card1.Element is ElementType.Water))
                return (player1, $"{player1.Name}'s {card1.Name} (Damage: {card1.Damage}) drowns {player2.Name}'s {card2.Name} (Damage: {card2.Damage}). {player1.Name} wins the round.");

            // Kraken is immune to spells
            if (card1.Type is CardType.Kraken && card2.Type is CardType.Spell)
                return (player1, $"{player1.Name}'s {card1.Name} (Damage: {card1.Damage}) is immune to {player2.Name}'s {card2.Name} (Damage: {card2.Damage}). {player1.Name} wins the round.");
            if (card2.Type is CardType.Kraken && card1.Type is CardType.Spell)
                return (player2, $"{player2.Name}'s {card2.Name} (Damage: {card2.Damage}) is immune to {player1.Name}'s {card1.Name} (Damage: {card1.Damage}). {player2.Name} wins the round.");

            // FireElf evades Dragon
            if ((card1.Element is ElementType.Fire && card1.Type is CardType.Elf) && card2.Type is CardType.Dragon)
                return (player1, $"{player1.Name}'s {card1.Name} (Damage: {card1.Damage}) evades {player2.Name}'s {card2.Name} (Damage: {card2.Damage})'s attack. {player1.Name} wins the round.");
            if ((card2.Element is ElementType.Fire && card2.Type is CardType.Elf) && card1.Type is CardType.Dragon)
                return (player2, $"{player2.Name}'s {card2.Name} (Damage: {card2.Damage}) evades {player1.Name}'s {card1.Name} (Damage: {card1.Damage})'s attack. {player2.Name} wins the round.");

            // Calculate base damage
            var card1Damage = card1.Damage;
            var card2Damage = card2.Damage;

            // Element-based damage adjustment
            if (card1.Type == CardType.Spell || card2.Type == CardType.Spell)
            {
                card1Damage = AdjustDamageForElement(card1, card2);
                card2Damage = AdjustDamageForElement(card2, card1);
            }

            // Compare damage
            if (card1Damage > card2Damage)
                return (player1, $"{player1.Name}'s {card1.Name} (Damage: {card1Damage}) overpowers {player2.Name}'s {card2.Name} (Damage: {card2Damage}). {player1.Name} wins the round.");
            if (card2Damage > card1Damage)
                return (player2, $"{player2.Name}'s {card2.Name} (Damage: {card2Damage}) overpowers {player1.Name}'s {card1.Name} (Damage: {card1Damage}). {player2.Name} wins the round.");
            
            // Draw
            return (null, $"{player1.Name}'s {card1.Name} (Damage: {card1Damage}) and {player2.Name}'s {card2.Name} (Damage: {card2Damage}) draw.");
        }

        public float AdjustDamageForElement(Card card1, Card card2)
        {
            return card1.Element switch
            {
                ElementType.Water when card2.Element == ElementType.Fire => card1.Damage * 2,
                ElementType.Fire when card2.Element == ElementType.Water => card1.Damage / 2,
                ElementType.Fire when card2.Element == ElementType.Normal => card1.Damage * 2,
                ElementType.Normal when card2.Element == ElementType.Fire => card1.Damage / 2,
                ElementType.Normal when card2.Element == ElementType.Water => card1.Damage * 2,
                ElementType.Water when card2.Element == ElementType.Normal => card1.Damage / 2,
                _ => card1.Damage
            };
        }
    }
}
