using Npgsql;
using MonsterTradingCardsGame.DTOs;
using MonsterTradingCardsGame.DAL.Exceptions;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private const string CreateTablesCommand = "CREATE TABLE IF NOT EXISTS Tokens ( TokenId SERIAL PRIMARY KEY, " +
                                                   "Username VARCHAR(50) UNIQUE NOT NULL, Token VARCHAR(255) UNIQUE NOT NULL);";

        private const string InsertTokenCommand = "INSERT INTO Tokens (Username, Token) VALUES (@Username, @Token)" +
                                                  " ON CONFLICT (Username) DO UPDATE SET Token = @Token;";

        private const string SelectTokenByUsernameCommand = "SELECT * FROM Tokens WHERE Username = @Username;";

        private readonly string _connectionString;

        public TokenRepository(string connectionString)
        {
            _connectionString = connectionString;
            EnsureTables();
        }

        private void EnsureTables()
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();
                using var command = new NpgsqlCommand(CreateTablesCommand, connection);
                command.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Could not connect to database", ex);
            }
        }

        public void SaveOrUpdateToken(TokenDTO userToken)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(InsertTokenCommand, connection);
            command.Parameters.AddWithValue("@Username", userToken.Username);
            command.Parameters.AddWithValue("@Token", userToken.Token);
            command.ExecuteNonQuery();
        }

        public string GetToken(string username)
        {
            try
            {
                using var connection = new NpgsqlConnection(_connectionString);
                connection.Open();

                using var command = new NpgsqlCommand(SelectTokenByUsernameCommand, connection);
                command.Parameters.AddWithValue("@Username", username);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    // Check if the token is null or empty
                    var token = reader["Token"].ToString();
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        throw new TokenNotFoundException($"Token is null or empty for username: {username}");
                    }

                    return token;
                }

                // No rows found for the given username
                throw new TokenNotFoundException($"No token found for username: {username}");
            }
            catch (TokenNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the token.", ex);
            }
        }

    }
}
