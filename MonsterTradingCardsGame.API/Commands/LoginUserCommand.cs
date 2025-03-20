using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Services;
using MonsterTradingCardsGame.DTOs;
using MonsterTradingCardsGame.BLL.Exceptions;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class LoginCommand : IRequestCommand
    {
        private readonly ITokenService _tokenService;

        public LoginCommand(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public HttpResponse Execute(HttpRequest request)
        {
            var response = new HttpResponse();

            try
            {
                // Ensure the request payload is not null
                if (string.IsNullOrWhiteSpace(request.Payload))
                {
                    throw new InvalidCredentialsException("Invalid username/password provided");
                }

                // Deserialize the payload into a DTO
                var userCredentials = System.Text.Json.JsonSerializer.Deserialize<UserCredentialsDTO>(request.Payload);
                if (userCredentials == null)
                {
                    throw new InvalidCredentialsException("Invalid username/password provided");
                }

                // Perform the login and generate a token
                var token = _tokenService.Login(userCredentials);

                response.StatusCode = StatusCode.Ok;
                response.Payload = System.Text.Json.JsonSerializer.Serialize(token);
                response.ContentType = "application/json; charset=utf-8";
            }
            catch (InvalidCredentialsException ex)
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
