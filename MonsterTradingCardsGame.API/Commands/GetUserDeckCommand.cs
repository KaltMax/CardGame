using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class GetUserDeckCommand : IRequestCommand
    {
        private readonly ICardService _cardService;
        private readonly ITokenService _tokenService;

        public GetUserDeckCommand(ICardService cardService, ITokenService tokenService)
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

                var deck = _cardService.GetUserDeck(targetUsername);

                if (request.ResourcePath.Contains("?format=plain"))
                {
                    var plainFormat = string.Join("\n", deck.Cards.Select(card => $"{card.Name} ({card.Damage})"));
                    response.StatusCode = StatusCode.Ok;
                    response.Payload = plainFormat;
                }
                else
                {
                    response.StatusCode = StatusCode.Ok;
                    response.Payload = System.Text.Json.JsonSerializer.Serialize(deck.Cards);
                    response.ContentType = "application/json; charset=utf-8";
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = $"401 Unauthorized: {ex.Message}";
            }
            catch (DeckRetrievalException ex)
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
    }

}
