using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public interface ITokenRepository
    {
        void SaveOrUpdateToken(TokenDTO userToken);
        string GetToken(string username);

    }
}
