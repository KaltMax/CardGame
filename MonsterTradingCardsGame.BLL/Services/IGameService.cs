using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.BLL.Services
{
    public interface IGameService
    {
        UserStatsDTO GetUserStats(string username);
        List<UserStatsDTO> GetScoreboard();
        Dictionary<string, string> EnterLobby(string username);
        string ConvertBattleLogToString(Dictionary<string, string> battleLog);
    }
}
