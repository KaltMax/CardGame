using MonsterTradingCardsGame.DAL.Repositories;
using MonsterTradingCardsGame.DTOs;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Models;
using MonsterTradingCardsGame.DAL.Exceptions;
using System.Text;

namespace MonsterTradingCardsGame.BLL.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly ICardRepository _cardRepository;
        private readonly Lobby _lobby;

        public GameService(IGameRepository gameRepository, ICardRepository cardRepository)
        {
            _gameRepository = gameRepository;
            _cardRepository = cardRepository;
            _lobby = new Lobby();
        }

        public UserStatsDTO GetUserStats(string username)
        {
            try
            {
                var userStats = _gameRepository.GetUserStats(username);

                return userStats;
            }
            catch (UserNotFoundException ex)
            {
                throw new UserDoesNotExistException("User not found", ex);
            }
            catch (Exception ex)
            {
                throw new StatsRetrievalException("Failed to Retrieve the Userstats.", ex);
            }

        }

        public List<UserStatsDTO> GetScoreboard()
        {
            try
            {
                var scoreboard = _gameRepository.GetScoreboard();

                return scoreboard;
            }
            catch (Exception ex)
            {
                throw new ScoreboardRetrievalException("Failed to Retrieve the Scoreboard.", ex);
            }
        }

        public Dictionary<string, string> EnterLobby(string username)
        {
            var player = GetUserForBattle(username);
            if (player == null)
            {
                throw new UserDoesNotExistException("User not found.");
            }

            var battleLog = _lobby.EnterLobby(player);

            if (battleLog == null)
            {
                throw new BattleFailedException("No battle log returned. Something went wrong.");
            }

            // Update the user stats in the database
            _gameRepository.UpdateUserStats(player.Name, player.Elo, player.Wins, player.Losses);

            return battleLog;
        }

        private User GetUserForBattle(string username)
        {
            try
            {
                // Assemble the User here
                var userStats = _gameRepository.GetUserStats(username);
                var userCards = _cardRepository.GetUserDeck(username);
                var userDeck = new Deck(userCards);

                var user = new User(userStats.Name, userDeck, userStats.Elo, userStats.Wins, userStats.Losses);

                return user;
            }
            catch (UserNotFoundException ex)
            {
                throw new UserDoesNotExistException("User not found", ex);
            }
            catch (Exception ex)
            {
                throw new UserForBattleRetrievalException("Failed to Retrieve the User.", ex);
            }
        }

        public string ConvertBattleLogToString(Dictionary<string, string> battleLog)
        {
            var convertedBattleLog = new StringBuilder();
            foreach (var logEntry in battleLog)
            {
                convertedBattleLog.AppendLine($"{logEntry.Key}:");
                convertedBattleLog.AppendLine($"{logEntry.Value}");
                convertedBattleLog.AppendLine(new string('-', 70));
            }

            return convertedBattleLog.ToString();
        }
    }
}
