using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Services
{
    public interface ICardService
    {
        List<CardDTO> GetUserCards(string username);
        UserDeckDTO GetUserDeck(string username);
        bool ConfigureUserDeck(string username, ConfigureDeckRequestDTO cardIds);
    }
}