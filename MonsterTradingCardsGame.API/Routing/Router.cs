using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.API.Commands;
using MonsterTradingCardsGame.BLL.Services;
using HttpMethod = MonsterTradingCardsGame.API.Server.HttpMethod;

namespace MonsterTradingCardsGame.API.Routing
{
    public class Router : IRouter
    {
        private readonly List<(string BasePath, HttpMethod Method, IRequestCommand Command)> _routes;

        public Router(IUserService userService, 
            ITokenService tokenService, 
            IGameService gameService, 
            ICardService cardService, 
            IPackageService packageService, 
            ITradingsService tradingsService, 
            IFriendsService friendsService)
        {
            // Initialize routes with base paths
            _routes = new List<(string, HttpMethod, IRequestCommand)>
            {
                ("/users", HttpMethod.Post, new RegisterUserCommand(userService)),
                ("/users/{username}", HttpMethod.Get, new GetUserDataCommand(userService, tokenService)),
                ("/users/{username}", HttpMethod.Put, new UpdateUserDataCommand(userService, tokenService)),
                ("/sessions", HttpMethod.Post, new LoginCommand(tokenService)),
                ("/cards", HttpMethod.Get, new GetUserCardsCommand(cardService, tokenService)),
                ("/deck", HttpMethod.Get, new GetUserDeckCommand(cardService, tokenService)),
                ("/deck", HttpMethod.Put, new ConfigureUserDeckCommand(cardService, tokenService)),
                ("/packages", HttpMethod.Post, new CreatePackageCommand(packageService, tokenService)),
                ("/transactions/packages", HttpMethod.Post, new AcquirePackageCommand(packageService, tokenService)),
                ("/stats", HttpMethod.Get, new GetUserStatsCommand(gameService, tokenService)),
                ("/scoreboard", HttpMethod.Get, new GetScoreboardCommand(gameService, tokenService)),
                ("/battles", HttpMethod.Post, new StartBattleCommand(gameService, tokenService)),
                ("/tradings", HttpMethod.Get, new GetAvailableTradingDealsCommand(tradingsService, tokenService)),
                ("/tradings", HttpMethod.Post, new CreateTradingDealCommand(tradingsService, tokenService)),
                ("/tradings/{tradingdealid}", HttpMethod.Delete, new DeleteTradingDealCommand(tradingsService, tokenService)),
                ("/tradings/{tradingdealid}", HttpMethod.Post, new ExecuteTradingDealCommand(tradingsService, tokenService)),
                ("/friends", HttpMethod.Get, new GetFriendsCommand(friendsService, tokenService)),
                ("/friends", HttpMethod.Post, new SendFriendRequestCommand(friendsService, tokenService)),
                ("/friends/{friendusername}", HttpMethod.Delete, new RemoveFriendCommand(friendsService, tokenService)),
                ("/friends/accept/{friendusername}", HttpMethod.Post, new AcceptFriendRequestCommand(friendsService, tokenService))
            };
        }

        public HttpResponse Route(HttpRequest request)
        {
            foreach (var (basePath, method, command) in _routes)
            {
                if (MatchesRoute(basePath, method, request.ResourcePath, request.Method))
                {
                    return command.Execute(request);
                }
            }

            return new HttpResponse
            {
                StatusCode = StatusCode.NotFound,
                Payload = "404 Not Found"
            };
        }

        private bool MatchesRoute(string basePath, HttpMethod expectedMethod, string resourcePath, HttpMethod actualMethod)
        {

            // Check if the HTTP method matches
            if (expectedMethod != actualMethod)
                return false;

            // Strip query parameters and trailing slashes
            var cleanResourcePath = resourcePath.Split('?')[0].Trim('/');

            // Split paths into segments
            var baseSegments = basePath.Trim('/').Split('/');
            var resourceSegments = cleanResourcePath.Split('/');

            // Check if the number of segments match
            if (baseSegments.Length != resourceSegments.Length)
                return false;

            for (int i = 0; i < baseSegments.Length; i++)
            {
                // Check if the segments are dynamic
                if (baseSegments[i].StartsWith("{") && baseSegments[i].EndsWith("}"))
                {
                    continue;
                }

                // Check if the segments are static
                if (baseSegments[i] != resourceSegments[i])
                {
                    return false; // Static segments must match
                }
            }

            return true;
        }
    }
}
