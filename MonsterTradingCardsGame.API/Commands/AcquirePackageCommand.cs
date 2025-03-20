using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class AcquirePackageCommand : IRequestCommand
    {
        private readonly IPackageService _packageService;
        private readonly ITokenService _tokenService;

        public AcquirePackageCommand(IPackageService packageService, ITokenService tokenService)
        {
            _packageService = packageService;
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

                var package = _packageService.AcquirePackage(username);

                response.StatusCode = StatusCode.Created;
                response.Payload = System.Text.Json.JsonSerializer.Serialize(package.Cards);
                response.ContentType = "application/json; charset=utf-8";
            }
            catch (UnauthorizedAccessException ex)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = $"401 Unauthorized: {ex.Message}";
            }
            catch (InsufficientFundsException ex)
            {
                response.StatusCode = StatusCode.Forbidden;
                response.Payload = $"403 Forbidden: {ex.Message}";
            }
            catch (NoPackagesAvailableException ex)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Payload = $"404 Not Found: {ex.Message}";
            }
            catch (UserDoesNotExistException ex)
            {
                response.StatusCode = StatusCode.NotFound;
                response.Payload = $"404 Not Found: {ex.Message}";
            }
            catch (AcquirePackageException ex)
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
