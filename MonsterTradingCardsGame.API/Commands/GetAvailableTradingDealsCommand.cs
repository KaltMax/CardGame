using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Services;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class GetAvailableTradingDealsCommand : IRequestCommand
    {
        private readonly ITradingsService _tradingsService;
        private readonly ITokenService _tokenService;

        public GetAvailableTradingDealsCommand(ITradingsService tradingsService, ITokenService tokenService)
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

                var tradingDeals = _tradingsService.GetAvailableTradingDeals();

                response.StatusCode = StatusCode.Ok;
                response.Payload = System.Text.Json.JsonSerializer.Serialize(tradingDeals);
                response.ContentType = "application/json; charset=utf-8";
            }
            catch (UnauthorizedAccessException ex)
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
