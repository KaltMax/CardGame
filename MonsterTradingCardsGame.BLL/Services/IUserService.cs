using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Services
{
    public interface IUserService
    {
        bool RegisterUser(string username, string password);
        UserDataDTO GetUserData(string username);
        bool UpdateUserData(string username, UserDataDTO newUserData);
    }
}
