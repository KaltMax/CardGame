using Npgsql;
using MonsterTradingCardsGame.DTOs;
using MonsterTradingCardsGame.DAL.Exceptions;

namespace MonsterTradingCardsGame.DAL.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        // SQL command strings:
        private const string CreatePackagesTableCommand = "CREATE TABLE IF NOT EXISTS Packages(" +
                                                    "PackageId SERIAL PRIMARY KEY," +
                                                    "IsAvailable BOOLEAN DEFAULT TRUE," +
                                                    "Card1Id VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE," +
                                                    "Card2Id VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE," +
                                                    "Card3Id VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE," +
                                                    "Card4Id VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE," +
                                                    "Card5Id VARCHAR(50) REFERENCES Cards(CardId) ON DELETE CASCADE);";

        private const string CreateCardsTableCommand = "CREATE TABLE IF NOT EXISTS Cards (" +
                                                       "CardId VARCHAR(50) PRIMARY KEY," +
                                                       "Name VARCHAR(100) NOT NULL," +
                                                       "Damage FLOAT NOT NULL," +
                                                       "UserId INT REFERENCES Users(UserId) ON DELETE SET NULL," +
                                                       "InDeck BOOLEAN DEFAULT FALSE);";

        private const string InsertCardCommand = "INSERT INTO Cards (CardId, Name, Damage) " +
                                                 "VALUES (@CardId, @Name, @Damage);";

        private const string InsertPackageCommand = "INSERT INTO Packages (Card1Id, Card2Id, Card3Id, Card4Id, Card5Id) " +
                                                    "VALUES (@Card1Id, @Card2Id, @Card3Id, @Card4Id, @Card5Id);";

        private const string SelectAvailablePackageCommand = "SELECT p.PackageId, p.Card1Id, p.Card2Id, p.Card3Id, p.Card4Id, p.Card5Id, " +
                                                             "c1.Name AS Card1Name, c1.Damage AS Card1Damage, " +
                                                             "c2.Name AS Card2Name, c2.Damage AS Card2Damage, " +
                                                             "c3.Name AS Card3Name, c3.Damage AS Card3Damage, " +
                                                             "c4.Name AS Card4Name, c4.Damage AS Card4Damage, " +
                                                             "c5.Name AS Card5Name, c5.Damage AS Card5Damage " +
                                                             "FROM Packages p " +
                                                             "JOIN Cards c1 ON p.Card1Id = c1.CardId " +
                                                             "JOIN Cards c2 ON p.Card2Id = c2.CardId " +
                                                             "JOIN Cards c3 ON p.Card3Id = c3.CardId " +
                                                             "JOIN Cards c4 ON p.Card4Id = c4.CardId " +
                                                             "JOIN Cards c5 ON p.Card5Id = c5.CardId " +
                                                             "WHERE IsAvailable = TRUE " +
                                                             "ORDER BY p.PackageId ASC " +
                                                             "LIMIT 1 " + // Only one package can be acquired at a time
                                                             "FOR UPDATE;"; // Lock the package for other transactions

        private const string UpdatePackageAvailabilityCommand = "UPDATE Packages SET IsAvailable = FALSE WHERE PackageId = @PackageId;";
        private const string UpdateCardOwnershipCommand = "UPDATE Cards SET UserId = @UserId WHERE CardId = ANY(@CardIds)";
        private const string GetCardsCommand = "SELECT Card1Id, Card2Id, Card3Id, Card4Id, Card5Id FROM Packages WHERE PackageId = @PackageId";
        private const string DeductUserCoinsCommand = "UPDATE Users SET Coins = Coins - @CoinAmount WHERE UserId = @UserId;";
        private const string SelectUserByUsernameCommand = "SELECT * FROM Users WHERE Username = @Username;";

        private readonly string _connectionString;

        public PackageRepository(string connectionString)
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
                // Create the packages table if not exists
                using var command = new NpgsqlCommand(CreatePackagesTableCommand, connection);
                command.ExecuteNonQuery();
                // Create the cards table if not exists
                using var cardCommand = new NpgsqlCommand(CreateCardsTableCommand, connection);
                cardCommand.ExecuteNonQuery();
            }
            catch (NpgsqlException ex)
            {
                throw new DataAccessException("Could not connect to database", ex);
            }
        }

        public bool AddPackage(PackageDTO package)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            try
            {
                // Insert each card into the Cards table
                foreach (var card in package.Cards)
                {
                    using var cardCommand = new NpgsqlCommand(InsertCardCommand, connection);
                    cardCommand.Parameters.AddWithValue("@CardId", card.Id);
                    cardCommand.Parameters.AddWithValue("@Name", card.Name);
                    cardCommand.Parameters.AddWithValue("@Damage", card.Damage);
                    cardCommand.ExecuteNonQuery();
                }

                // Insert the package into Packages table
                using var packageCommand = new NpgsqlCommand(InsertPackageCommand, connection);
                packageCommand.Parameters.AddWithValue("@PackageId", Guid.NewGuid().ToString());
                packageCommand.Parameters.AddWithValue("@Card1Id", package.Cards[0].Id);
                packageCommand.Parameters.AddWithValue("@Card2Id", package.Cards[1].Id);
                packageCommand.Parameters.AddWithValue("@Card3Id", package.Cards[2].Id);
                packageCommand.Parameters.AddWithValue("@Card4Id", package.Cards[3].Id);
                packageCommand.Parameters.AddWithValue("@Card5Id", package.Cards[4].Id);
                packageCommand.ExecuteNonQuery();

                transaction.Commit();
                return true;
            }
            catch (PostgresException ex) when (ex.SqlState == "23505") // Unique violation
            {
                transaction.Rollback();
                throw new AddPackageException("At least one card in the package already exists");
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public PackageDTO AcquirePackageAndDeductCoins(string username, decimal packagePrice)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();
            var userId = GetUserId(username, connection);
            using var transaction = connection.BeginTransaction();

            try
            {
                // Retrieve the current balance of the user
                var currentBalance = GetUserCoins(username, connection, transaction);

                // Check if the user has enough coins
                if (currentBalance < packagePrice)
                {
                    throw new NotEnoughCoinsException("Not enough money for buying a card package");
                }

                // Lock and retrieve an available package
                var (packageId, cards) = SelectAvailablePackage(connection, transaction);
                if (packageId == 0)
                {
                    throw new PackageUnavailableException("No card package available for buying");
                }

                // Deduct coins from the user's balance
                if (!DeductUserCoins(userId, packagePrice, connection, transaction))
                {
                    throw new InvalidOperationException("Failed to deduct user coins.");
                }

                // Update card ownership and mark the package as unavailable
                if (!UpdateCardsOwnership(packageId, userId, connection, transaction) ||
                    !MarkPackageAsUnavailable(packageId, connection, transaction))
                {
                    throw new InvalidOperationException("Failed to complete package acquisition.");
                }

                transaction.Commit();
                return new PackageDTO(cards);
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        private (int packageId, List<CardDTO> cards) SelectAvailablePackage(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            using var selectCommand = new NpgsqlCommand(SelectAvailablePackageCommand, connection, transaction);
            using var reader = selectCommand.ExecuteReader();

            if (!reader.Read())
            {
                throw new PackageUnavailableException("No card package available for buying");
            }

            var packageId = Convert.ToInt32(reader["PackageId"]);
            var cards = new List<CardDTO>
            {
                new(reader["Card1Id"].ToString() ?? throw new InvalidOperationException(), reader["Card1Name"].ToString() ?? throw new InvalidOperationException(), Convert.ToSingle(reader["Card1Damage"])),
                new(reader["Card2Id"].ToString() ?? throw new InvalidOperationException(), reader["Card2Name"].ToString() ?? throw new InvalidOperationException(), Convert.ToSingle(reader["Card2Damage"])),
                new(reader["Card3Id"].ToString() ?? throw new InvalidOperationException(), reader["Card3Name"].ToString() ?? throw new InvalidOperationException(), Convert.ToSingle(reader["Card3Damage"])),
                new(reader["Card4Id"].ToString() ?? throw new InvalidOperationException(), reader["Card4Name"].ToString() ?? throw new InvalidOperationException(), Convert.ToSingle(reader["Card4Damage"])),
                new(reader["Card5Id"].ToString() ?? throw new InvalidOperationException(), reader["Card5Name"].ToString() ?? throw new InvalidOperationException(), Convert.ToSingle(reader["Card5Damage"]))
            };
            reader.Close();

            return (packageId, cards);
        }

        private int GetUserCoins(string username, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            using var command = new NpgsqlCommand(SelectUserByUsernameCommand, connection, transaction);
            command.Parameters.AddWithValue("@Username", username);
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return Convert.ToInt32(reader["Coins"]);
            }

            throw new UserNotFoundException("User not found.");
        }


        private bool DeductUserCoins(int userId, decimal coinAmount, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            using var command = new NpgsqlCommand(DeductUserCoinsCommand, connection, transaction);
            command.Parameters.AddWithValue("@CoinAmount", coinAmount);
            command.Parameters.AddWithValue("@UserId", userId);
            return command.ExecuteNonQuery() > 0;
        }

        private bool UpdateCardsOwnership(int packageId, int userId, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            // Step 1: Retrieve card IDs for the specified package
            using var cardsCommand = new NpgsqlCommand(GetCardsCommand, connection, transaction);
            cardsCommand.Parameters.AddWithValue("PackageId", packageId);

            var cardIds = new List<string>();
            using (var reader = cardsCommand.ExecuteReader())
            {
                if (reader.Read())
                {
                    cardIds.Add(reader["Card1Id"].ToString() ?? throw new InvalidOperationException());
                    cardIds.Add(reader["Card2Id"].ToString() ?? throw new InvalidOperationException());
                    cardIds.Add(reader["Card3Id"].ToString() ?? throw new InvalidOperationException());
                    cardIds.Add(reader["Card4Id"].ToString() ?? throw new InvalidOperationException());
                    cardIds.Add(reader["Card5Id"].ToString() ?? throw new InvalidOperationException());
                }
            }

            // Step 2: Update the UserId for these cards
            using var updateCommand = new NpgsqlCommand(UpdateCardOwnershipCommand, connection, transaction);
            updateCommand.Parameters.AddWithValue("@UserId", userId);
            updateCommand.Parameters.AddWithValue("@CardIds", cardIds.ToArray());

            return updateCommand.ExecuteNonQuery() == cardIds.Count;
        }

        private bool MarkPackageAsUnavailable(int packageId, NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            using var updateCommand = new NpgsqlCommand(UpdatePackageAvailabilityCommand, connection, transaction);
            updateCommand.Parameters.AddWithValue("@PackageId", packageId);

            return updateCommand.ExecuteNonQuery() == 1;
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
