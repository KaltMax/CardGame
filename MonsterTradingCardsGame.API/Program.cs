using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.API.Routing;
using MonsterTradingCardsGame.BLL.Services;
using MonsterTradingCardsGame.DAL.Repositories;

namespace MonsterTradingCardsGame.API
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string connectionString = "Host=localhost;Port=5432;Username=max;Password=postgres;Database=cardgame";

            // Repositories
            var gameRepository = new GameRepository(connectionString);
            var cardRepository = new CardRepository(connectionString);
            var packageRepository = new PackageRepository(connectionString);
            var tokenRepository = new TokenRepository(connectionString);
            var tradingsRepository = new TradingsRepository(connectionString);
            var userRepository = new UserRepository(connectionString);
            var friendsRepository = new FriendsRepository(connectionString);

            // Services
            var cardService = new CardService(cardRepository);
            var gameService = new GameService(gameRepository, cardRepository);
            var packageService = new PackageService(packageRepository);
            var tokenService = new TokenService(tokenRepository, userRepository);
            var tradingsService = new TradingsService(tradingsRepository, cardRepository);
            var userService = new UserService(userRepository);
            var friendsService = new FriendsService(friendsRepository);

            // Router
            var router = new Router(userService, tokenService, gameService, cardService, packageService, tradingsService, friendsService);

            // Start the HTTP server
            const int port = 10001;
            var server = new HttpServer(port, router);

            try
            {
                server.Start();
                Console.WriteLine($"Server started on port {port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error starting server: {ex.Message}");
            }
        }
    }
}
