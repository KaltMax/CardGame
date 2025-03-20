using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Services;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class GetFriendsCommand : IRequestCommand
    {
        private readonly IFriendsService _friendsService;
        private readonly ITokenService _tokenService;

        public GetFriendsCommand(IFriendsService friendsService, ITokenService tokenService)
        {
            _friendsService = friendsService;
            _tokenService = tokenService;
        }

        public HttpResponse Execute(HttpRequest request)
        {
            var response = new HttpResponse();

            try
            {
                var authorizationHeader = request.Header.GetValueOrDefault("Authorization");
                var username = _tokenService.GetUsernameFromToken(authorizationHeader);
                _tokenService.ValidateToken(authorizationHeader, username);

                var userData = _friendsService.GetFriendsList(username);

                response.StatusCode = StatusCode.Ok;
                response.Payload = System.Text.Json.JsonSerializer.Serialize(userData);
                response.ContentType = "application/json; charset=utf-8";
            }
            catch (UnauthorizedAccessException ex)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = $"401 Unauthorized: {ex.Message}";
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.InternalServerError;
                response.Payload = $"500 Internal Server Error: {ex.Message}";
            }

            return response;
        }
    }
}
