namespace MonsterTradingCardsGame.DTOs
{
    public class UserDeckDTO
    {
        public List<CardDTO> Cards { get; init; }

        public UserDeckDTO()
        {
            Cards = new List<CardDTO>();
        }

        public UserDeckDTO(List<CardDTO> cards)
        {
            if (cards.Count > 4)
                throw new ArgumentException("A deck can only contain up to 4 cards.");

            Cards = cards;
        }
    }
}
