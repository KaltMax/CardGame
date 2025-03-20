using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public interface IGameRepository
    {
        UserStatsDTO GetUserStats(string username);
        List<UserStatsDTO> GetScoreboard();
        bool UpdateUserStats(string name, int elo, int wins, int losses);
    }
}
