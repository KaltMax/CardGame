using MonsterTradingCardsGame.API.Exceptions;
using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class SendFriendRequestCommand : IRequestCommand
    {
        private readonly IFriendsService _friendsService;
        private readonly ITokenService _tokenService;

        public SendFriendRequestCommand(IFriendsService friendsService, ITokenService tokenService)
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

                if (string.IsNullOrWhiteSpace(request.Payload))
                {
                    throw new InvalidFriendRequestDataException("Missing user name payload.");
                }

                var targetUsername = request.Payload;

                _friendsService.SendFriendRequest(username, targetUsername);

                response.StatusCode = StatusCode.Created;
                response.Payload = "Friend request sent successfully.";
            }
            catch (ForbiddenAccessException ex)
            {
                response.StatusCode = StatusCode.Forbidden;
                response.Payload = $"403 Forbidden: {ex.Message}";
            }
            catch (FriendShipAlreadyExistsException ex)
            {
                response.StatusCode = StatusCode.Conflict;
                response.Payload = $"409 Conflict: {ex.Message}";
            }
            catch (UnauthorizedAccessException ex)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = $"401 Unauthorized: {ex.Message}";
            }
            catch (InvalidFriendRequestDataException ex)
            {
                response.StatusCode = StatusCode.BadRequest;
                response.Payload = $"400 Bad Request: {ex.Message}";
            }
            catch (UserDoesNotExistException ex)
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
