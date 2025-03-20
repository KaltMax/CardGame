using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public interface IUserRepository
    {
        bool UserAlreadyExists(string username);
        bool AddUser(string username, string password);
        UserDataDTO GetUserData(string username);
        UserCredentialsDTO GetUserCredentials(string username);
        bool UpdateUserData(string username, UserDataDTO newUserData);
    }
}
