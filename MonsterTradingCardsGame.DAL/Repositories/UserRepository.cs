using Npgsql;
using MonsterTradingCardsGame.DTOs;
using MonsterTradingCardsGame.DAL.Exceptions;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        // SQL command strings:
        private const string CreateTablesCommand =
            "CREATE TABLE IF NOT EXISTS Users (UserId SERIAL PRIMARY KEY,Username VARCHAR(50) NOT NULL UNIQUE," +
            "Password VARCHAR(100) NOT NULL,Coins DECIMAL,Elo INT, Wins INT,Losses INT," +
            "Name VARCHAR(100),Bio TEXT,ProfilePicture TEXT);";

        private const string InsertUserCommand =
            "INSERT INTO Users (Username, Password, Coins, Elo, Wins, Losses, Name, Bio, " +
            "ProfilePicture)VALUES (@Username, @Password, @Coins, @Elo, @Wins, @Losses, @Name, " +
            "@Bio, @ProfilePicture);";

        private const string SelectUserByUsernameCommand = "SELECT * FROM Users WHERE Username = @Username;";

        private const string UpdateUserDataCommand =
            "UPDATE Users SET Name = @Name, Bio = @Bio, ProfilePicture = @ProfilePicture WHERE Username = @Username";

        private const string GetUserCredentialsCommand =
            "SELECT Username, Password FROM Users WHERE Username = @Username;";

        private readonly string _connectionString;

        // Default values of a new User
        private const decimal InitialCoins = 20;
        private const int InitialWins = 0;
        private const int InitialLosses = 0;
        private const int InitialElo = 100;
        private const string InitialBio = "No bio available";
        private const string InitialImage = "No Image";

        public UserRepository(string connectionString)
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

        public bool UserAlreadyExists(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(SelectUserByUsernameCommand, connection);
            command.Parameters.AddWithValue("@Username", username);
            using var reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public bool AddUser(string username, string password)
        {
            if (UserAlreadyExists(username))
            {
                throw new UsernameAlreadyExistsException("User with same username already registered");
            }

            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(InsertUserCommand, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            command.Parameters.AddWithValue("@Coins", InitialCoins);
            command.Parameters.AddWithValue("@Elo", InitialElo);
            command.Parameters.AddWithValue("@Wins", InitialWins);
            command.Parameters.AddWithValue("@Losses", InitialLosses);
            command.Parameters.AddWithValue("@Name", username);
            command.Parameters.AddWithValue("@Bio", InitialBio);
            command.Parameters.AddWithValue("@ProfilePicture", InitialImage);

            return command.ExecuteNonQuery() == 1;
        }

        public UserDataDTO GetUserData(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(SelectUserByUsernameCommand, connection);
            command.Parameters.AddWithValue("@Username", username);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new UserDataDTO(
                    reader["Name"].ToString() ?? throw new InvalidOperationException(),
                    reader["Bio"].ToString() ?? throw new InvalidOperationException(),
                    reader["ProfilePicture"].ToString() ?? throw new InvalidOperationException()
                );
            }

            throw new UserNotFoundException("User not found.");
        }

        public UserCredentialsDTO GetUserCredentials(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(GetUserCredentialsCommand, connection);
            command.Parameters.AddWithValue("@Username", username);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new UserCredentialsDTO(
                    reader["Username"].ToString() ?? throw new InvalidOperationException(),
                    reader["Password"].ToString() ?? throw new InvalidOperationException()
                );
            }

            throw new UserNotFoundException("User not found.");
        }

        public bool UpdateUserData(string username, UserDataDTO newUserData)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(UpdateUserDataCommand, connection);
            command.Parameters.AddWithValue("@Name", newUserData.Name);
            command.Parameters.AddWithValue("@Bio", newUserData.Bio);
            command.Parameters.AddWithValue("@ProfilePicture", newUserData.Image);
            command.Parameters.AddWithValue("@Username", username);

            return command.ExecuteNonQuery() > 0;
        }
    }
}
