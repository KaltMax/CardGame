using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;
using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class CreateTradingDealCommand : IRequestCommand
    {
        private readonly ITradingsService _tradingsService;
        private readonly ITokenService _tokenService;

        public CreateTradingDealCommand(ITradingsService tradingsService, ITokenService tokenService)
        {
            _tradingsService = tradingsService;
            _tokenService = tokenService;
        }

        public HttpResponse Execute(HttpRequest request)
        {
            var response = new HttpResponse();

            try
            {
                // Authenticate the user
                var authorizationHeader = request.Header.GetValueOrDefault("Authorization");
                var username = _tokenService.GetUsernameFromToken(authorizationHeader);
                _tokenService.ValidateToken(authorizationHeader, username);

                if (string.IsNullOrWhiteSpace(request.Payload))
                {
                    throw new InvalidTradingDealException("Request payload is empty.");
                }

                var tradingDeal = ParseTradingDealRequest(request.Payload);

                _tradingsService.CreateTradingDeal(username, tradingDeal);

                response.StatusCode = StatusCode.Created;
                response.Payload = "201 Created: Trading deal successfully created.";
            }
            catch (UnauthorizedAccessException ex)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = $"401 Unauthorized: {ex.Message}";
            }
            catch (InvalidTradingDealException ex)
            {
                response.StatusCode = StatusCode.BadRequest;
                response.Payload = $"400 Bad Request: {ex.Message}";
            }
            catch (CardNotAvailableException ex)
            {
                response.StatusCode = StatusCode.Forbidden;
                response.Payload = $"403 Forbidden: {ex.Message}";
            }
            catch (TradingDealAlreadyExistsException ex)
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

        public TradingDealDTO ParseTradingDealRequest(string payload)
        {
            return System.Text.Json.JsonSerializer.Deserialize<TradingDealDTO>(payload)
                   ?? throw new InvalidTradingDealException("Invalid trading deal format.");
        }
    }

}
