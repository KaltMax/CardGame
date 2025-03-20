using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Services;
using MonsterTradingCardsGame.BLL.Exceptions;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class DeleteTradingDealCommand : IRequestCommand
    {
        private readonly ITradingsService _tradingsService;
        private readonly ITokenService _tokenService;

        public DeleteTradingDealCommand(ITradingsService tradingsService, ITokenService tokenService)
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

                var tradingDealId = request.ResourcePath.Split('/').Last();
                _tradingsService.DeleteTradingDeal(username, tradingDealId);

                response.StatusCode = StatusCode.Ok;
                response.Payload = "200 OK: Trading deal successfully deleted.";
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
            catch (ForbiddenAccessException ex)
            {
                response.StatusCode = StatusCode.Forbidden;
                response.Payload = $"403 Forbidden: {ex.Message}";
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
