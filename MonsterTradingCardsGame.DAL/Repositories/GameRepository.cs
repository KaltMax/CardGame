using Npgsql;
using MonsterTradingCardsGame.DTOs;
using MonsterTradingCardsGame.DAL.Exceptions;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly string _connectionString;

        private const string GetUserStatsCommand =
            "SELECT Name, Elo, Wins, Losses FROM Users WHERE Username = @Username;";

        private const string GetScoreboardCommand =
            "SELECT Name, Elo, Wins, Losses FROM Users WHERE Username != 'admin' ORDER BY Elo DESC;";

        private const string UpdateUserStatsCommand =
            "UPDATE Users SET Elo = @Elo, Wins = @Wins, Losses = @Losses WHERE Name = @Name;";

        public GameRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public UserStatsDTO GetUserStats(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(GetUserStatsCommand, connection);
            command.Parameters.AddWithValue("@Username", username);
            using var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                throw new UserNotFoundException();
            }

            var name = reader.GetString(reader.GetOrdinal("Name"));
            var elo = reader.GetInt32(reader.GetOrdinal("Elo"));
            var wins = reader.GetInt32(reader.GetOrdinal("Wins"));
            var losses = reader.GetInt32(reader.GetOrdinal("Losses"));

            return new UserStatsDTO(
                name: name,
                elo: elo,
                wins: wins,
                losses: losses
            );
        }

        public List<UserStatsDTO> GetScoreboard()
        {
            var scoreboard = new List<UserStatsDTO>();

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(GetScoreboardCommand, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var name = reader.GetString(reader.GetOrdinal("Name"));
                var elo = reader.GetInt32(reader.GetOrdinal("Elo"));
                var wins = reader.GetInt32(reader.GetOrdinal("Wins"));
                var losses = reader.GetInt32(reader.GetOrdinal("Losses"));

                scoreboard.Add(new UserStatsDTO(
                    name: name,
                    elo: elo,
                    wins: wins,
                    losses: losses
                ));
            }
            return scoreboard;
        }

        public bool UpdateUserStats(string name, int elo, int wins, int losses)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                using var command = new NpgsqlCommand(UpdateUserStatsCommand, connection);
                command.Parameters.AddWithValue("@Elo", elo);
                command.Parameters.AddWithValue("@Wins", wins);
                command.Parameters.AddWithValue("@Losses", losses);
                command.Parameters.AddWithValue("@Name", name ?? throw new InvalidOperationException());
                return command.ExecuteNonQuery() > 0;
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to update user stats in the database.", ex);
            }
            catch (Exception ex)
            {
                throw new UserStatsUpdateException("An unexpected error occurred while updating user stats.", ex);
            }
        }
    }
}
