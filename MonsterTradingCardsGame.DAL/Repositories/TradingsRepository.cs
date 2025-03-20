using MonsterTradingCardsGame.DAL.Exceptions;
using MonsterTradingCardsGame.DTOs;
using Npgsql;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public class TradingsRepository : ITradingsRepository
    {
        private readonly string _connectionString;

        private const string CreateTradesTableCommand = "CREATE TABLE IF NOT EXISTS Trades (" +
                                                        "TradeId VARCHAR(50) PRIMARY KEY," +
                                                        "OfferingUserId INT REFERENCES Users(UserId) ON DELETE CASCADE," +
                                                        "CardToTradeId VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE," +
                                                        "RequestedCardType VARCHAR(50)," +
                                                        "RequestedMinDamage FLOAT," +
                                                        "IsTradeActive BOOLEAN DEFAULT TRUE);";

        private const string SaveTradingDealCommand = "INSERT INTO Trades (TradeId, OfferingUserId, CardToTradeId, RequestedCardType, RequestedMinDamage) " +
                                                      "VALUES (@TradeId, @OfferingUserId, @CardToTradeId, @RequestedCardType, @RequestedMinDamage);";

        private const string DeleteTradingDealCommand = "DELETE FROM Trades WHERE TradeId = @TradeId;";

        private const string RetrieveAvailableTradingDealsCommand = "SELECT * FROM Trades WHERE IsTradeActive = TRUE;";

        private const string SelectUserByUsernameCommand = "SELECT * FROM Users WHERE Username = @Username;";

        private const string GetOfferingUserIdCommand = "SELECT OfferingUserId FROM Trades WHERE TradeId = @TradeId;";

        private const string GetTradingDealCommand = "SELECT * FROM Trades WHERE TradeId = @TradeId;";

        private const string UpdateCardsOwnerShipCommand = "UPDATE Cards SET UserId = @NewOwnerId WHERE CardId = @CardId AND UserId = @CurrentOwnerId;";

        private const string MarkTradeAsInactiveCommand = "UPDATE Trades SET IsTradeActive = FALSE WHERE TradeId = @TradeId;";

        private const string CheckTradingDealExistsCommand = "SELECT COUNT(*) FROM Trades WHERE TradeId = @TradeId;";

        private const string CheckTradingDealBelongsToUserCommand = "SELECT COUNT(*) FROM Trades WHERE TradeId = @TradeId AND OfferingUserId = @OfferingUserId;";

        public TradingsRepository(string connectionString)
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
                // Create trades table if it does not exist
                using var command = new NpgsqlCommand(CreateTradesTableCommand, connection);
                command.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Could not connect to database", ex);
            }
        }

        public void SaveTradingDeal(string username, TradingDealDTO tradingDeal)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            // 1. Check if a trade with this TradeId already exists => throw if so
            if (TradingDealExists(tradingDeal.Id, connection))
            {
                throw new TradingDealIdAlreadyTakenException("A deal with this deal ID already exists.");
            }

            // 2. Get the userId for the offering user
            var userId = GetUserId(username);

            // 3. Insert the new trade
            using var command = new NpgsqlCommand(SaveTradingDealCommand, connection);
            command.Parameters.AddWithValue("@TradeId", tradingDeal.Id);
            command.Parameters.AddWithValue("@OfferingUserId", userId);
            command.Parameters.AddWithValue("@CardToTradeId", tradingDeal.CardToTrade);
            command.Parameters.AddWithValue("@RequestedCardType", tradingDeal.Type);
            command.Parameters.AddWithValue("@RequestedMinDamage", tradingDeal.MinimumDamage);
            command.ExecuteNonQuery();
        }

        public List<TradingDealDTO> GetAvailableTradingDeals()
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(RetrieveAvailableTradingDealsCommand, connection);
            using var reader = command.ExecuteReader();

            var tradingDeals = new List<TradingDealDTO>();
            while (reader.Read())
            {
                tradingDeals.Add(new TradingDealDTO(
                    reader["TradeId"].ToString() ?? throw new InvalidOperationException(),
                    reader["CardToTradeId"].ToString() ?? throw new InvalidOperationException(),
                    reader["RequestedCardType"].ToString() ?? throw new InvalidOperationException(),
                    Convert.ToSingle(reader["RequestedMinDamage"])
                ));
            }

            return tradingDeals;
        }

        public void DeleteTradingDeal(string username, string tradeId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            // 1. Check if the trade exists => throw if not
            if (!TradingDealExists(tradeId, connection))
            {
                throw new TradingDealDoesNotExistException("The provided deal ID was not found.");
            }

            // 2. Get the userId for the offering user
            var userId = GetUserId(username);

            // 3. Check if the trade was created by the user => throw if not
            if (!TradingDealBelongsToUser(tradeId, userId, connection))
            {
                throw new CardNotFromUserException("The deal contains a card that is not owned by the user.");
            }

            // 4. Delete the trade
            using var command = new NpgsqlCommand(DeleteTradingDealCommand, connection);
            command.Parameters.AddWithValue("@TradeId", tradeId);
            command.ExecuteNonQuery();
        }

        private bool TradingDealExists(string tradeId, NpgsqlConnection connection)
        {
            using var command = new NpgsqlCommand(CheckTradingDealExistsCommand, connection);
            command.Parameters.AddWithValue("@TradeId", tradeId);
            var count = (long)command.ExecuteScalar()!;
            return count > 0;
        }

        private bool TradingDealBelongsToUser(string tradeId, int userId, NpgsqlConnection connection)
        {
            using var command = new NpgsqlCommand(CheckTradingDealBelongsToUserCommand, connection);
            command.Parameters.AddWithValue("@TradeId", tradeId);
            command.Parameters.AddWithValue("@OfferingUserId", userId);
            var count = (long)command.ExecuteScalar()!;
            return count > 0;
        }

        public TradingDealDTO GetTradingDealById(string tradeId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(GetTradingDealCommand, connection);
            command.Parameters.AddWithValue("TradeId", tradeId);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new TradingDealDTO(
                    reader["TradeId"].ToString() ?? throw new InvalidOperationException(),
                    reader["CardToTradeId"].ToString() ?? throw new InvalidOperationException(),
                    reader["RequestedCardType"].ToString() ?? throw new InvalidOperationException(),
                    Convert.ToSingle(reader["RequestedMinDamage"])
                );
            }

            throw new TradingDealDoesNotExistException("The provided deal ID was not found.");
        }

        public void TradingDealTransaction(string username, int buyerUserId, int offeringUserId, string tradingDealId, string offeredCardId, TradingDealDTO tradingDeal)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                // Remove the offered card from the buyer
                TransferCardOwnership(offeredCardId, buyerUserId, offeringUserId, connection, transaction);

                // STransfer the card in the trading deal to the buyer
                TransferCardOwnership(tradingDeal.CardToTrade, offeringUserId, buyerUserId, connection, transaction);

                // Mark the trading deal as inactive
                MarkTradeAsInactive(tradingDealId, connection, transaction);

                // Commit the transaction
                transaction.Commit();
            }
            catch (Exception ex)
            {
                // Rollback transaction on any error
                transaction.Rollback();
                throw new DataAccessException("An error occurred while executing the trading deal.", ex);
            }
        }

        private void TransferCardOwnership(string cardId, int currentOwnerId, int newOwnerId, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            using var command = new NpgsqlCommand(UpdateCardsOwnerShipCommand, connection, transaction);
            command.Parameters.AddWithValue("@CardId", cardId);
            command.Parameters.AddWithValue("@CurrentOwnerId", currentOwnerId);
            command.Parameters.AddWithValue("@NewOwnerId", newOwnerId);
            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                throw new CardNotFromUserException("Failed to transfer ownership of the card.");
            }
        }

        private void MarkTradeAsInactive(string tradeId, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            using var command = new NpgsqlCommand(MarkTradeAsInactiveCommand, connection, transaction);
            command.Parameters.AddWithValue("@TradeId", tradeId);

            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
            {
                throw new TradingDealDoesNotExistException("Failed to mark the trade as inactive.");
            }
        }

        public int GetOfferingUserId(string tradingDealId)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            using var command = new NpgsqlCommand(GetOfferingUserIdCommand, connection);
            command.Parameters.AddWithValue("@TradeId", tradingDealId);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return Convert.ToInt32(reader["OfferingUserId"]);
            }
            throw new TradingDealDoesNotExistException("The provided deal ID was not found.");
        }

        public int GetUserId(string username)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
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
