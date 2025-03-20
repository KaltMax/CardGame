using MonsterTradingCardsGame.API.Exceptions;
using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;
using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class UpdateUserDataCommand : IRequestCommand
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public UpdateUserDataCommand(IUserService userService, ITokenService tokenService)
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
                _tokenService.ValidateToken(authorizationHeader, targetUsername);

                // Ensure payload exists and is not empty
                if (string.IsNullOrWhiteSpace(request.Payload))
                {
                    throw new InvalidUserDataException("Missing user data payload.");
                }

                // Parse the new user data (will throw an exception if invalid)
                var newUserData = ParseUserData(request.Payload);

                // Update user data
                var result = _userService.UpdateUserData(targetUsername, newUserData);

                if (result)
                {
                    response.StatusCode = StatusCode.Ok;
                    response.Payload = "200 OK: User successfully updated";
                }
                else
                {
                    throw new Exception("An error occurred while updating user data.");
                }

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
            catch (InvalidUserDataException ex)
            {
                response.StatusCode = StatusCode.BadRequest;
                response.Payload = $"400 Bad Request: {ex.Message}";
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.InternalServerError;
                response.Payload = $"500 Internal Server Error: {ex.Message}";
            }

            return response;
        }

        public UserDataDTO ParseUserData(string requestBody)
        {
            var result = System.Text.Json.JsonSerializer.Deserialize<UserDataDTO>(requestBody);
            if (result == null)
            {
                throw new InvalidUserDataException("Invalid user data format.");
            }
            return result;
        }
    }

}
