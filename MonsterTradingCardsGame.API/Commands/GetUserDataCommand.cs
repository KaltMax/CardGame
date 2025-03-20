using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class GetUserDataCommand : IRequestCommand
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public GetUserDataCommand(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        public HttpResponse Execute(HttpRequest request)
        {
            var response = new HttpResponse();

            try
            {
                var targetUsername = request.ResourcePath.Split('/').Last();
                var authorizationHeader = request.Header.GetValueOrDefault("Authorization");

                if (string.IsNullOrWhiteSpace(authorizationHeader))
                {
                    response.StatusCode = StatusCode.Unauthorized;
                    response.Payload = "401 Unauthorized: Authorization header is missing.";
                    return response;
                }

                _tokenService.ValidateToken(authorizationHeader, targetUsername);

                var userData = _userService.GetUserData(targetUsername);

                response.StatusCode = StatusCode.Ok;
                response.Payload = System.Text.Json.JsonSerializer.Serialize(userData);
                response.ContentType = "application/json; charset=utf-8";
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
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.InternalServerError;
                response.Payload = $"500 Internal Server Error: {ex.Message}";
            }

            return response;
        }
    }
}
