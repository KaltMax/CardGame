using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Services
{
    public interface ITokenService
    {
        string Login(UserCredentialsDTO userCredentials);
        void ValidateToken(string? authorizationHeader, string targetUsername);
        void ValidateAdminToken(string? authorizationHeader);
        string GetUsernameFromToken(string? authorizationHeader);
    }
}
