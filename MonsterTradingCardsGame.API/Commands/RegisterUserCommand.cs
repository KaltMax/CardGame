using MonsterTradingCardsGame.API.Exceptions;
using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;
using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class RegisterUserCommand : IRequestCommand
    {
        private readonly IUserService _userService;

        public RegisterUserCommand(IUserService userService)
        {
            _userService = userService;
        }

        public HttpResponse Execute(HttpRequest request)
        {
            var response = new HttpResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(request.Payload))
                {
                    throw new InvalidUserDataException("User data is missing.");
                }

                var userCredentials = System.Text.Json.JsonSerializer.Deserialize<UserCredentialsDTO>(request.Payload);
                if (userCredentials == null)
                {
                    throw new InvalidUserDataException("Invalid user credentials format.");
                }

                _userService.RegisterUser(userCredentials.Username, userCredentials.Password);

                response.StatusCode = StatusCode.Created;
                response.Payload = "201 Created: User successfully created";
            }
            catch (InvalidUserDataException ex)
            {
                response.StatusCode = StatusCode.BadRequest;
                response.Payload = $"400 Bad Request: {ex.Message}";
            }
            catch (UsernameAlreadyTaken ex)
            {
                response.StatusCode = StatusCode.Conflict;
                response.Payload = $"409 Conflict: {ex.Message}";
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
