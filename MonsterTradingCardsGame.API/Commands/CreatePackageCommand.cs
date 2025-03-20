using MonsterTradingCardsGame.API.Exceptions;
using MonsterTradingCardsGame.API.Server;
using MonsterTradingCardsGame.BLL.Exceptions;
using MonsterTradingCardsGame.BLL.Services;
using MonsterTradingCardsGame.DTOs;

namespace MonsterTradingCardsGame.API.Commands
{
    internal class CreatePackageCommand : IRequestCommand
    {
        private readonly IPackageService _packageService;
        private readonly ITokenService _tokenService;

        public CreatePackageCommand(IPackageService packageService, ITokenService tokenService)
        {
            _packageService = packageService;
            _tokenService = tokenService;
        }

        public HttpResponse Execute(HttpRequest request)
        {
            var response = new HttpResponse();

            try
            {
                if (string.IsNullOrWhiteSpace(request.Payload))
                {
                    throw new InvalidPackageDataException("Package data is missing.");
                }

                // Use the same ParsePackage logic from your old controller
                var package = ParsePackage(request.Payload);

                if (package.Cards.Count != 5)
                {
                    throw new InvalidPackageDataException("A package must contain exactly 5 cards.");
                }

                var authorizationHeader = request.Header.GetValueOrDefault("Authorization");
                _tokenService.ValidateAdminToken(authorizationHeader);

                _packageService.CreatePackage(package);

                response.StatusCode = StatusCode.Created;
                response.Payload = "201 Created: Package and cards successfully created.";
            }
            catch (UnauthorizedAccessException ex)
            {
                response.StatusCode = StatusCode.Unauthorized;
                response.Payload = $"401 Unauthorized: {ex.Message}";
            }
            catch (InvalidPackageDataException ex)
            {
                response.StatusCode = StatusCode.BadRequest;
                response.Payload = $"400 Bad Request: {ex.Message}";
            }
            catch (PackageCreationException ex)
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

        private PackageDTO ParsePackage(string requestBody)
        {
            var cards = System.Text.Json.JsonSerializer.Deserialize<List<CardDTO>>(requestBody);
            return new PackageDTO(cards ?? throw new InvalidPackageDataException());
        }
    }
}
