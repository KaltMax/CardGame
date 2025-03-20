using MonsterTradingCardsGame.API.Exceptions;
using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;
using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class ConfigureUserDeckCommand : IRequestCommand
    {
        private readonly ICardService _cardService;
        private readonly ITokenService _tokenService;

        public ConfigureUserDeckCommand(ICardService cardService, ITokenService tokenService)
        {
            _cardService = cardService;
            _tokenService = tokenService;
        }

        public HttpResponse Execute(HttpRequest request)
        {
            var response = new HttpResponse();

            try
            {
                var authorizationHeader = request.Header.GetValueOrDefault("Authorization");
                var targetUsername = _tokenService.GetUsernameFromToken(authorizationHeader);
                _tokenService.ValidateToken(authorizationHeader, targetUsername);

                if (string.IsNullOrWhiteSpace(request.Payload))
                {
                    throw new InvalidDeckConfigurationException("The request doesn't contain a deck.");
                }

                // Use the ParseDeckRequest method to handle deserialization
                var deckRequest = ParseDeckRequest(request.Payload);

                if (deckRequest.CardIds.Count != 4)
                {
                    throw new InvalidDeckConfigurationException("The provided deck must include exactly 4 cards.");
                }

                if (_cardService.ConfigureUserDeck(targetUsername, deckRequest))
                {
                    response.StatusCode = StatusCode.Ok;
                    response.Payload = "The deck has been successfully configured.";
                }
                else
                {
                    throw new ConfigureDeckException("The deck configuration failed due to an unknown error.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = $"401 Unauthorized: {ex.Message}";
            }
            catch (InvalidDeckConfigurationException ex)
            {
                response.StatusCode = StatusCode.BadRequest;
                response.Payload = $"400 Bad Request: {ex.Message}";
            }
            catch (CardNotAvailableException ex)
            {
                response.StatusCode = StatusCode.Forbidden;
                response.Payload = $"403 Forbidden: {ex.Message}";
            }
            catch (ConfigureDeckException ex)
            {
                response.StatusCode = StatusCode.InternalServerError;
                response.Payload = $"500 Internal Server Error: {ex.Message}";
            }
            catch (Exception ex)
            {
                response.StatusCode = StatusCode.InternalServerError;
                response.Payload = $"500 Internal Server Error: {ex.Message}";
            }

            return response;
        }

        private ConfigureDeckRequestDTO ParseDeckRequest(string requestBody)
        {
            var cardIds = System.Text.Json.JsonSerializer.Deserialize<List<string>>(requestBody);
            if (cardIds == null || cardIds.Count == 0)
            {
                throw new InvalidDeckConfigurationException("Invalid deck request format.");
            }

            return new ConfigureDeckRequestDTO(cardIds);
        }
    }
}
