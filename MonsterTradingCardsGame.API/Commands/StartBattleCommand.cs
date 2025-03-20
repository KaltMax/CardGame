using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class StartBattleCommand : IRequestCommand
    {
        private readonly IGameService _gameService;
        private readonly ITokenService _tokenService;

        public StartBattleCommand(IGameService gameService, ITokenService tokenService)
        {
            _gameService = gameService;
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

                var battleResult = _gameService.EnterLobby(username);

                response.StatusCode = StatusCode.Ok;
                response.Payload = _gameService.ConvertBattleLogToString(battleResult);
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
            catch (BattleFailedException ex)
            {
                response.StatusCode = StatusCode.InternalServerError;
                response.Payload = $"500 Internal Server Error: {ex.Message}";
            }
            catch (PlayerAlreadyInLobbyOrBattleException ex)
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
