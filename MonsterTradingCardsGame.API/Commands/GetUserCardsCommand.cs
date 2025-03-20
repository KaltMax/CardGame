using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class GetUserCardsCommand : IRequestCommand
    {
        private readonly ICardService _cardService;
        private readonly ITokenService _tokenService;

        public GetUserCardsCommand(ICardService cardService, ITokenService tokenService)
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

                var cards = _cardService.GetUserCards(targetUsername);

                response.StatusCode = StatusCode.Ok;
                response.Payload = System.Text.Json.JsonSerializer.Serialize(cards);
                response.ContentType = "application/json; charset=utf-8";
            }
            catch (UnauthorizedAccessException ex)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = $"401 Unauthorized: {ex.Message}";
            }
            catch (CardRetrievalException ex)
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
