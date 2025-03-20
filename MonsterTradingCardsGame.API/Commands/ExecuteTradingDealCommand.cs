using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Services;
using MonsterTradingCardsGame.BLL.Exceptions;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class ExecuteTradingDealCommand : IRequestCommand
    {
        private readonly ITradingsService _tradingsService;
        private readonly ITokenService _tokenService;

        public ExecuteTradingDealCommand(ITradingsService tradingsService, ITokenService tokenService)
        {
            _tradingsService = tradingsService;
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
                    throw new InvalidTradingDealException("Offered card ID is missing.");
                }

                var offeredCardId = System.Text.Json.JsonSerializer.Deserialize<string>(request.Payload);
                var tradingDealId = request.ResourcePath.Split('/').Last();

                if (offeredCardId != null) _tradingsService.ExecuteTradingDeal(username, tradingDealId, offeredCardId);

                response.StatusCode = StatusCode.Created;
                response.Payload = "201 Created: Trading deal successfully executed.";
            }
            catch (UnauthorizedAccessException ex)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = $"401 Unauthorized: {ex.Message}";
            }
            catch (TradingDealNotFoundException ex)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Payload = $"404 Not Found: {ex.Message}";
            }
            catch (TradingRequirementsNotMetException)
            {
                response.StatusCode = StatusCode.Forbidden;
                response.Payload = "403 Forbidden: The trading requirements are not met.";
            }
            catch (ForbiddenAccessException ex)
            {
                response.StatusCode = StatusCode.Forbidden;
                response.Payload = $"403 Forbidden: {ex.Message}";
            }
            catch (InvalidTradingDealException ex)
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
    }
}
