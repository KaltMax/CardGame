using MonsterTradingCardsGame.DTOs;
using Npgsql;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public interface ICardRepository
    {
        List<CardDTO> GetUserCards(string username);
        UserDeckDTO GetUserDeck(string username);
        bool ConfigureUserDeck(string username, ConfigureDeckRequestDTO cardIds);
        void CardExists(string cardId);
        CardDTO GetCardById(string id);
    }
}
