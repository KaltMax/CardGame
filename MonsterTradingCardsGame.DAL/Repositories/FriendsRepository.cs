using MonsterTradingCardsGame.DTOs;
using MonsterTradingCardsGame.DAL.Exceptions;
using Npgsql;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public class FriendsRepository : IFriendsRepository
    {
        private readonly string _connectionString;

        private const string CreateTableCommand = 
            "CREATE TABLE IF NOT EXISTS Friends (FriendshipId SERIAL PRIMARY KEY, UserId1 INT REFERENCES Users(UserId) ON DELETE CASCADE, " +
            "UserId2 INT REFERENCES Users(UserId) ON DELETE CASCADE, Status VARCHAR(20) DEFAULT 'pending');";

        private const string GetFriendsCommand =
            "SELECT u1.Username AS Username1, u2.Username AS Username2, f.Status FROM Friends f JOIN Users u1 ON f.UserId1 = u1.UserId " +
            "JOIN Users u2 ON f.UserId2 = u2.UserId WHERE u1.Username = @Username OR u2.Username = @Username;";

       private const string CheckExistingFriendRequestCommand =
            "SELECT COUNT(*) FROM Friends " +
            "WHERE (UserId1 = @UserId1 AND UserId2 = @UserId2) " +
            "   OR (UserId1 = @UserId2 AND UserId2 = @UserId1);";

        private const string InsertFriendRequestCommand =
            "INSERT INTO Friends (UserId1, UserId2, Status) " +
            "VALUES (@UserId1, @UserId2, 'pending');";

        private const string CheckPendingRequestAndSenderCommand =
            "SELECT UserId1 FROM Friends " +
            "WHERE ((UserId1 = @UserId1 AND UserId2 = @UserId2) " +
            "OR (UserId1 = @UserId2 AND UserId2 = @UserId1)) " +
            "AND Status = 'pending';";

        private const string UpdateFriendRequestCommand =
            "UPDATE Friends " +
            "SET Status = 'accepted' " +
            "WHERE ((UserId1 = @UserId1 AND UserId2 = @UserId2) " +
            "   OR (UserId1 = @UserId2 AND UserId2 = @UserId1)) " +
            "   AND Status = 'pending';";

        private const string CheckFriendshipExistsCommand =
            "SELECT COUNT(*) FROM Friends " +
            "WHERE (UserId1 = @UserId1 AND UserId2 = @UserId2) " +
            "   OR (UserId1 = @UserId2 AND UserId2 = @UserId1);";

        private const string DeleteFriendshipCommand =
            "DELETE FROM Friends " +
            "WHERE (UserId1 = @UserId1 AND UserId2 = @UserId2) " +
            "   OR (UserId1 = @UserId2 AND UserId2 = @UserId1);";

        private const string SelectUserByUsernameCommand = "SELECT * FROM Users WHERE Username = @Username;";

        public FriendsRepository(string connectionString)
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
                using var command = new NpgsqlCommand(CreateTableCommand, connection);
                command.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Could not connect to database", ex);
            }
        }

        public List<FriendEntryDTO> RetrieveFriends(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(GetFriendsCommand, connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = command.ExecuteReader();

            var friendsList = new List<FriendEntryDTO>();

            try
            {
                while (reader.Read())
                {
                    // Determine the friend username (not the same as the current user)
                    var friendUsername = reader.GetString(reader.GetOrdinal("Username1")) == username
                        ? reader.GetString(reader.GetOrdinal("Username2"))
                        : reader.GetString(reader.GetOrdinal("Username1"));

                    var status = reader.GetString(reader.GetOrdinal("Status"));

                    // Add a new FriendEntryDTO to the list
                    friendsList.Add(new FriendEntryDTO( friendUsername, status));
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to retrieve friends from the database.", ex);
            }

            return friendsList;
        }

        public void CreateFriendRequest(string username, string targetUsername)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction(); // Start transaction
            try
            {
                var userId1 = GetUserId(username, connection);
                var userId2 = GetUserId(targetUsername, connection);

                if (userId1 == userId2)
                {
                    throw new SelfFriendRequestException("Users cannot send friend requests to themselves.");
                }

                using (var checkCommand = new NpgsqlCommand(CheckExistingFriendRequestCommand, connection, transaction))
                {
                    checkCommand.Parameters.AddWithValue("@UserId1", userId1);
                    checkCommand.Parameters.AddWithValue("@UserId2", userId2);

                    var existingRequests = (long)checkCommand.ExecuteScalar()!;
                    if (existingRequests > 0)
                    {
                        throw new DuplicateFriendRequestException("A friendship request already exists between these users.");
                    }
                }

                using (var insertCommand = new NpgsqlCommand(InsertFriendRequestCommand, connection, transaction))
                {
                    insertCommand.Parameters.AddWithValue("@UserId1", userId1);
                    insertCommand.Parameters.AddWithValue("@UserId2", userId2);
                    insertCommand.ExecuteNonQuery();
                }

                transaction.Commit(); 
            }
            catch (Exception)
            {
                transaction.Rollback(); 
                throw;
            }
        }

        public void MarkFriendRequestAsAccepted(string username, string targetUsername)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction(); // Start transaction
            try
            {
                var userId1 = GetUserId(username, connection);
                var userId2 = GetUserId(targetUsername, connection);

                using (var checkCommand = new NpgsqlCommand(CheckPendingRequestAndSenderCommand, connection, transaction))
                {
                    checkCommand.Parameters.AddWithValue("@UserId1", userId1);
                    checkCommand.Parameters.AddWithValue("@UserId2", userId2);

                    var creatorId = checkCommand.ExecuteScalar() as int?;

                    if (creatorId == null)
                    {
                        throw new FriendRequestNotFoundException("No pending friend request found.");
                    }

                    if (creatorId == userId1)
                    {
                        throw new ForbiddenFriendException("The sender of the friend request cannot accept it.");
                    }
                }

                using (var updateCommand = new NpgsqlCommand(UpdateFriendRequestCommand, connection, transaction))
                {
                    updateCommand.Parameters.AddWithValue("@UserId1", userId1);
                    updateCommand.Parameters.AddWithValue("@UserId2", userId2);
                    updateCommand.ExecuteNonQuery();
                }

                transaction.Commit(); // Commit transaction if all operations succeed
            }
            catch (Exception)
            {
                transaction.Rollback(); // Rollback transaction on error
                throw;
            }
        }

        public void DeleteFriendship(string username, string targetUsername)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction(); // Start transaction
            try
            {
                var userId1 = GetUserId(username, connection);
                var userId2 = GetUserId(targetUsername, connection);

                using (var checkCommand = new NpgsqlCommand(CheckFriendshipExistsCommand, connection, transaction))
                {
                    checkCommand.Parameters.AddWithValue("@UserId1", userId1);
                    checkCommand.Parameters.AddWithValue("@UserId2", userId2);

                    var friendshipExists = (long)checkCommand.ExecuteScalar()!;
                    if (friendshipExists == 0)
                    {
                        throw new FriendRequestNotFoundException($"No friendship exists between {username} and {targetUsername}.");
                    }
                }

                using (var deleteCommand = new NpgsqlCommand(DeleteFriendshipCommand, connection, transaction))
                {
                    deleteCommand.Parameters.AddWithValue("@UserId1", userId1);
                    deleteCommand.Parameters.AddWithValue("@UserId2", userId2);
                    deleteCommand.ExecuteNonQuery();
                }

                transaction.Commit(); 
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        private int GetUserId(string username, NpgsqlConnection connection)
        {
            using var command = new NpgsqlCommand(SelectUserByUsernameCommand, connection);
            command.Parameters.AddWithValue("@Username", username);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return Convert.ToInt32(reader["UserId"]);
            }

            throw new UserNotFoundException("User not found.");
        }

    }
}
