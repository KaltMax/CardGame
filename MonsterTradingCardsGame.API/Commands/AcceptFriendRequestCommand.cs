using MonsterTradingCardsGame.API.Exceptions;
using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class AcceptFriendRequestCommand : IRequestCommand
    {
        private readonly IFriendsService _friendsService;
        private readonly ITokenService _tokenService;

        public AcceptFriendRequestCommand(IFriendsService friendsService, ITokenService tokenService)
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

                // Extract target username from the URL path (last segment)
                var segments = request.ResourcePath.Split('/');
                var targetUsername = segments.Length > 1 ? segments.Last() : null;

                if (targetUsername == null)
                {
                    throw new InvalidFriendRequestDataException("Missing target username.");
                }

                _friendsService.AcceptFriendRequest(username, targetUsername);

                response.StatusCode = StatusCode.Ok;
                response.Payload = "200 Friend request successfully accepted.";
            }
            catch (ForbiddenAccessException ex)
            {
                response.StatusCode = StatusCode.Forbidden;
                response.Payload = $"403 Forbidden: {ex.Message}";
            }
            catch (UnauthorizedAccessException ex)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = $"401 Unauthorized: {ex.Message}";
            }
            catch (UserDoesNotExistException ex)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Payload = $"404 Not Found: {ex.Message}";
            }
            catch (FriendRequestDoesNotExistException ex)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Payload = $"404 Not Found: {ex.Message}";
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
