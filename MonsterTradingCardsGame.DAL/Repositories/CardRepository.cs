using MonsterTradingCardsGame.DAL.Exceptions;
using MonsterTradingCardsGame.DTOs;
using Npgsql;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public class CardRepository : ICardRepository
    {
        private const string CreateCardsTableCommand = "CREATE TABLE IF NOT EXISTS Cards (" +
                                                       "CardId VARCHAR(50) PRIMARY KEY," +
                                                       "Name VARCHAR(100) NOT NULL," +
                                                       "Damage FLOAT NOT NULL," +
                                                       "UserId INT REFERENCES Users(UserId) ON DELETE SET NULL," +
                                                       "InDeck BOOLEAN DEFAULT FALSE);";

        private const string SelectUserByUsernameCommand = "SELECT * FROM Users WHERE Username = @Username;";

        private const string GetUserCardsCommand = "SELECT * FROM Cards WHERE UserId = @UserId;";

        private const string GetUserDeckCommand = "SELECT * FROM Cards WHERE UserId = @UserId AND InDeck = TRUE;";

        private const string ClearUserDeckCommand = "UPDATE Cards SET InDeck = FALSE WHERE UserId = @UserId;";

        private const string AddCardToDeckCommand = "UPDATE Cards SET InDeck = TRUE WHERE UserId = @UserId AND CardId = @CardId;";

        private const string GetAllCardsCommand = "SELECT COUNT(*) FROM Cards WHERE CardId = @CardId;";

        private const string GetCardByIdCommand = "SELECT * FROM Cards WHERE CardId = @CardId;";

        private readonly string _connectionString;


        public CardRepository(string connectionString)
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
                // Create cards table if it does not exist
                using var command = new NpgsqlCommand(CreateCardsTableCommand, connection);
                command.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Could not connect to database", ex);
            }
        }

        public List<CardDTO> GetUserCards(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            var userId = GetUserId(username, connection);
            using var command = new NpgsqlCommand(GetUserCardsCommand, connection);
            command.Parameters.AddWithValue("@UserId", userId);
            using var reader = command.ExecuteReader();

            var cards = new List<CardDTO>();

            try
            {
                while (reader.Read())
                {
                    var cardId = reader.GetString(reader.GetOrdinal("CardId"));
                    var name = reader.GetString(reader.GetOrdinal("Name"));
                    var damage = reader.GetFloat(reader.GetOrdinal("Damage"));

                    cards.Add(new CardDTO(cardId, name, damage));
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to retrieve user cards from the database", ex);
            }

            return cards;
        }

        public UserDeckDTO GetUserDeck(string username)
        {
            var deck = new UserDeckDTO();
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            var userId = GetUserId(username, connection);
            using var command = new NpgsqlCommand(GetUserDeckCommand, connection);
            command.Parameters.AddWithValue("@UserId", userId);

            using var reader = command.ExecuteReader();

            try
            {
                while (reader.Read())
                {
                    var cardId = reader.GetString(reader.GetOrdinal("CardId"));
                    var name = reader.GetString(reader.GetOrdinal("Name"));
                    var damage = reader.GetFloat(reader.GetOrdinal("Damage"));

                    var card = new CardDTO(cardId, name, damage);
                    deck.Cards.Add(card);
                }
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Failed to retrieve user deck from database", ex);
            }

            return deck;
        }

        public bool ConfigureUserDeck(string username, ConfigureDeckRequestDTO cardIds)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            var userId = GetUserId(username, connection);
            using var transaction = connection.BeginTransaction();
            try
            {
                // Step 1: Clear the user's deck
                using (var clearCommand = new NpgsqlCommand(ClearUserDeckCommand, connection, transaction))
                {
                    clearCommand.Parameters.AddWithValue("@UserId", userId);
                    clearCommand.ExecuteNonQuery();
                }

                // Step 2: Add each card to the deck
                foreach (var cardId in cardIds.CardIds)
                {
                    using var addCommand = new NpgsqlCommand(AddCardToDeckCommand, connection, transaction);
                    addCommand.Parameters.AddWithValue("@UserId", userId);
                    addCommand.Parameters.AddWithValue("@CardId", cardId);

                    var rowsAffected = addCommand.ExecuteNonQuery();
                    if (rowsAffected != 1)
                    {
                        throw new CardNotFromUserException($"Card {cardId} does not belong to user {userId} or does not exist.");
                    }
                }

                // Step 3: Commit the transaction if all operations succeed
                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                // Ensure the transaction is rolled back in case of any exception
                if (transaction.Connection != null)
                {
                    transaction.Rollback();
                }
                throw;
            }
        }

        public void CardExists(string cardId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(GetAllCardsCommand, connection);
            command.Parameters.AddWithValue("@CardId", cardId);
            var count = (long)(command.ExecuteScalar() ?? throw new InvalidOperationException());
            if (count != 1)
            {
                throw new CardDoesNotExistException("Card does not exist.");
            }
        }

        public CardDTO GetCardById(string id)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var command = new NpgsqlCommand(GetCardByIdCommand, connection);
            command.Parameters.AddWithValue("@CardId", id);
            using var reader = command.ExecuteReader();

            if (reader.Read())
            {
                var cardId = reader.GetString(reader.GetOrdinal("CardId"));
                var name = reader.GetString(reader.GetOrdinal("Name"));
                var damage = reader.GetFloat(reader.GetOrdinal("Damage"));

                return new CardDTO(cardId, name, damage);
            }

            throw new CardDoesNotExistException("Card does not exist.");
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
