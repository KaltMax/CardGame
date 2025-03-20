namespace MonsterTradingCardsGame.DTOs
{
    public class PackageDTO
    {
        public List<CardDTO> Cards { get; init; }

        public PackageDTO(List<CardDTO> cards)
        {
            // Ensuring that there are exactly 5 cards in each package
            if (cards.Count != 5)
            {
                throw new ArgumentException("A package must contain exactly 5 cards.");
            }
            Cards = cards;
        }
    }
}