using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Models
{
    public class Deck
    {
        public List<Card> PlayerDeck { get; init; }

        public Deck(UserDeckDTO playerDeck)
        {
            if (playerDeck?.Cards == null || playerDeck.Cards.Count == 0)
            {
                throw new ArgumentException("The provided deck is null or empty.");
            }

            PlayerDeck = ConvertCardsToBattleDeck(playerDeck);
        }

        public Deck()
        {
            PlayerDeck = new List<Card>();
        }

        public List<Card> ConvertCardsToBattleDeck(UserDeckDTO playerDeck)
        {
            // Convert each CardDTO into a Card using the existing Card constructor
            return playerDeck.Cards.Select(cardDto => new Card(cardDto)).ToList();
        }

        public void AddCard(Card card)
        {
            PlayerDeck.Add(card);
        }

        public void RemoveCard(Card card)
        {
            PlayerDeck.Remove(card);
        }
    }
}
